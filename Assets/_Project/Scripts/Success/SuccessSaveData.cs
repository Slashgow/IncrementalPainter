using System;
using System.Collections.Generic;

[Serializable]
public class SuccessSaveData
{
    public List<bool> successCompletionStates;
    public SuccessStatData successStatData;

    public SuccessSaveData()
    {
        successCompletionStates = new List<bool>();
        successStatData = new SuccessStatData();
    }

    public SuccessSaveData(SuccessStatData successStatData, List<SuccessData> allSuccessData)
    {
        this.successStatData = successStatData;

        successCompletionStates = new List<bool>();
        foreach (SuccessData success in allSuccessData)
        {
            successCompletionStates.Add(success.isDone);
        }
    }

}
