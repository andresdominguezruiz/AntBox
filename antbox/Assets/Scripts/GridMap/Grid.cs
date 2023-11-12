using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int height;
    private int width;
    private int[,] gridArray;

    public Grid(int height,int width){
        this.height=height;
        this.width=width;
        this.gridArray=new int[height,width];
    }
}
