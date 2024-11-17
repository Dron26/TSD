using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.TrolleyGroup;
using UnityEngine;

[RequireComponent(typeof(CVSLoader), typeof(SheetProcessor))]
public class GoogleSheetLoader : MonoBehaviour
{
    public event Action<bool, WebData,string> SucsessLoadedCertificate;
    
    [SerializeField] private WebData _data;
    
    private CVSLoader _cvsLoader;
    private SheetProcessor _sheetProcessor;

    public void StartDownload()
    {
        _cvsLoader = GetComponent<CVSLoader>();
        _sheetProcessor = GetComponent<SheetProcessor>();
        DownloadTable();
    }

    private void DownloadTable()
    {
        _cvsLoader.DownloadTable(OnRawCVSLoaded);
    }

    private void OnRawCVSLoaded(string rawCVSText)
    {
        bool isOk = false;
        
        if ( _sheetProcessor.CheckBuyedProgramm(rawCVSText))
        {
            _data = _sheetProcessor.ProcessData(rawCVSText);
            isOk = true;
        }

        SucsessLoadedCertificate?.Invoke(isOk,_data,_sheetProcessor.GetErrorMessage());
    }
}

[Serializable]
public class WebData
{
    public List<Employee> Employees;
    public List<Box> Boxes;
    public List<Trolley> Trolleys;
    
    public override string ToString()
    {
        string result = "";
        Employees.ForEach(o =>
        {
            result += o.ToString();
        });
        return result;
    }
}