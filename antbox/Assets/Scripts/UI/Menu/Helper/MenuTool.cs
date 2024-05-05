using System.Collections.Generic;

public static class MenuTool
{


    private static float minX = -5.5f;

    private static float maxX = 7f;

    private static float minY = -3.5f;
    private static float maxY = 3f;

    private static float speed = 0.5f;

    public static float MinX { get => minX; set => minX = value; }
    public static float MaxX { get => maxX; set => maxX = value; }
    public static float MinY { get => minY; set => minY = value; }
    public static float MaxY { get => maxY; set => maxY = value; }
    public static float Speed { get => speed; set => speed = value; }

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
