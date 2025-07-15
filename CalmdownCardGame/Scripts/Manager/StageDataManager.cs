using UnityEngine;

public class StageDataManager : MonoBehaviour
{
    public static StageDataManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadAllStages();
    }

    public StageData[] allStages;

    private void LoadAllStages()
    {
        allStages = Resources.LoadAll<StageData>("Stages");
        for(int i=0; i<allStages.Length; i++)
        {
            for(int j=0; j<allStages.Length-i-1; j++)
            {
                if(allStages[j].stageID > allStages[j+1].stageID)
                {
                    StageData temp = allStages[j];
                    allStages[j] = allStages[j+1];
                    allStages[j+1] = temp;
                }
            }
        }
    }

    public StageData GetStageByID(int id)
    {
        foreach(StageData stage in allStages)
        {
            if(stage.stageID == id)
            {
                return stage;
            }
        }
        return null;
    }
}
