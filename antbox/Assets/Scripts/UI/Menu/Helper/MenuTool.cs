using System.Collections.Generic;

public static class MenuTool
{

    public static void UpdaterTool(AntStats ant,List<AntStats> list){
        SelectableItem item=ant.gameObject.GetComponent<SelectableItem>();
        if(list.Contains(ant)){
            list.Remove(ant);
            if(item!=null){
                item.ChangeColorWithoutSelecting();
            }
        }
        else{
            list.Add(ant);
            if(item!=null){
                item.ChangeColorWithoutSelecting();
            }
        }
    }
    public static void UpdaterTool(AntStats ant,List<AntStats> list, bool condition){
        SelectableItem item=ant.gameObject.GetComponent<SelectableItem>();
        if(list.Contains(ant)){
            list.Remove(ant);
            if(item!=null){
                item.ChangeColorWithoutSelecting();
            }
        }
        else if(condition){
            list.Add(ant);
            if(item!=null){
                item.ChangeColorWithoutSelecting();
            }
        }
    }

}
