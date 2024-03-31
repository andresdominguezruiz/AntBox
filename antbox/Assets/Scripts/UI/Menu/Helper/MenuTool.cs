using System.Collections.Generic;

public class MenuTool
{
    public static void UpdaterTool(AntStats ant,List<AntStats> list, bool condition=true){
        SelectableItem item=ant.gameObject.GetComponent<SelectableItem>();
        if(list.Contains(ant)){
            list.Remove(ant);
            if(item!=null) item.ChangeColorWithoutSelecting();
        }
        else if(condition){
            list.Add(ant);
            if(item!=null) item.ChangeColorWithoutSelecting();
        }
    }

}
