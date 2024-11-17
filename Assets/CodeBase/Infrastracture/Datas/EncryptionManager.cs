using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class EncryptionManager 
{
    private const int KeyExpiryDays = 30;
    private byte[] _key;
    private DateTime _keyCreationTime;
    private const string Key = "1B2C3D4E5F708192A3B4C5D6E7F80112"; 
    private static readonly byte[] Iv = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

    public void CheckKey()
    {


        LoadKey();
    
         if (IsKeyExpired())
         {
            // DeleteExpiredKey();
            // LoadKey();
         }
        
         ValidateKey();
    }

    private void LoadKey()
    {
        string keyFilePath = Path.Combine(Application.persistentDataPath, "key.txt");

        try
        {
            if (File.Exists(keyFilePath))
            {
                string base64EncryptedKey = File.ReadAllText(keyFilePath);
                byte[] encryptedKeyBytes = Convert.FromBase64String(base64EncryptedKey);

                if (encryptedKeyBytes.Length > 0)
                {
                    _key = DecryptKey(encryptedKeyBytes);

                    // Попробуем декодировать ключ с использованием других кодировок
                    string decodedKeyUTF16 = Encoding.Unicode.GetString(_key);
                    string decodedKeyASCII = Encoding.ASCII.GetString(_key);

                    Debug.Log("Decoded Key (UTF-16): " + decodedKeyUTF16);
                    Debug.Log("Decoded Key (ASCII): " + decodedKeyASCII);
                    string decodedKeyUTF8 = Encoding.UTF8.GetString(_key);
                    Debug.Log("Decoded Key (UTF-8): " + decodedKeyUTF8);
                    // Теперь у вас есть декодированный ключ в виде строки
                }
                else
                {
                    Debug.LogError("Key file is empty. Unable to continue.");
                    Application.Quit();
                }
            }
            else
            {
                Debug.LogError("Key file not found. Unable to continue.");
                Application.Quit();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading key: {ex.Message}");
            Application.Quit();
        }
    }


    private bool IsKeyExpired()
    {
        TimeSpan expiryTime = DateTime.Now - _keyCreationTime;
        return expiryTime.TotalDays >= KeyExpiryDays;
    }

    private void DeleteExpiredKey()
    {
        string keyFilePath = Path.Combine(Application.persistentDataPath, "key.txt");
        try
        {
            File.Delete(keyFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error deleting expired key: {ex.Message}");
            Application.Quit();
        }
    }

    private void ValidateKey()
    {
        // Ожидаемое значение ключа в виде строки
        string expectedKeyString = "Hello World";
    
        // Преобразуем ожидаемую строку ключа в массив байтов
        byte[] expectedKeyBytes = Encoding.UTF8.GetBytes(expectedKeyString);
    
        // Сравниваем полученный ключ с ожидаемым значением
        if (!AreKeysEqual(_key, expectedKeyBytes))
        {
            Debug.LogError("Invalid key");
            Application.Quit();
        }
    }

    private byte[] DecryptKey(byte[] encryptedKey)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = new byte[aes.BlockSize / 8]; // Инициализация IV

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(encryptedKey))
            {
                // Прочитать IV из зашифрованных данных
                aes.IV = new byte[aes.BlockSize / 8];
                ms.Read(aes.IV, 0, aes.IV.Length);

                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream decryptedMs = new MemoryStream())
                    {
                        cs.CopyTo(decryptedMs);
                        return decryptedMs.ToArray();
                    }
                }
            }
        }
    }
    public static string EncryptKey(string keyToEncrypt)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(keyToEncrypt);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(keyBytes, 0, keyBytes.Length);
                }

                byte[] encryptedKeyBytes = ms.ToArray();
                return Convert.ToBase64String(encryptedKeyBytes);
            }
        }
    }
    private bool AreKeysEqual(byte[] key1, byte[] key2)
    {
        if (key1.Length != key2.Length)
        {
            return false;
        }

        for (int i = 0; i < key1.Length; i++)
        {
            if (key1[i] != key2[i])
            {
                return false;
            }
        }

        return true;
    }
}