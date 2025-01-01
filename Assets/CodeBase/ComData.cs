public class ComData 
{
    public int ComAddress;
    public int BaudeRate;
    public int ReadTimeout;
    public int WriteTimeout;
    public int Parity;
    public int DataBits;
    public int StopBits;
    
    public ComData(int comAddress, int baudeRate, int readTimeout, int writeTimeout, int parity, int dataBits, int stopBits)
    {
        ComAddress = comAddress;
        BaudeRate = baudeRate;
        ReadTimeout = readTimeout;
        WriteTimeout = writeTimeout;
        Parity = parity;
        DataBits = dataBits;
        StopBits = stopBits;
    }
}