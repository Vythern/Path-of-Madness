using UnityEngine;
using System.Collections.Generic;

public class Cell : MonoBehaviour
{
    [Header("Tiles")]
    [SerializeField] public GameObject[] floorTiles;
    [SerializeField] public GameObject[] ceilingTiles;

    private float generationBudget = 0f;
    public GameObject leftCell = null;
    public GameObject rightCell = null;

    [SerializeField] private List<GameObject> validExtras = new List<GameObject>();
    [SerializeField] private List<GameObject> validRightCells = new List<GameObject>();
    public int validRightCellCount = 0; //Do not modify in inspector
    
    private float minimumCost = 10000f;

    /*public enum TrapType
    {
        LeftSpikes,
        RightSpikes,
        DownSpikes,
        UpSpikes,
        Lava,
        Turret,
        SwingingBall,
        SawbladeUp,
        SawbladeDown,
        SawbladeLeft,
        SawbladeRight,
        SawbladeUpLeft,
        SawbladeUpRight,
        SawbladeDownLeft,
        SawbladeDownRight
    }*/
     
    //Game manager will generate new cells at runtime.  
    //It needs to know the rightmost active cell at any given time, and reference that cell's list of valid "next cells".  
    public GameObject getRandomValidCell()
    {
        int cellChoice = Random.Range(0, validRightCells.Count);
        return validRightCells[cellChoice];
    }

    void Start()
    {
        
    }

    public void destroyCell()
    {
        if(rightCell != null) 
        {
            this.rightCell.gameObject.GetComponent<Cell>().leftCell = null; //set the left cell of the destroyed cell to null.  
        }
        GameObject.Destroy(this.gameObject);
    }

    private void generateExtras()
    {
        //generate a random order of integers from 0 to validExtras.count instead of looping through the given list of traps as provided.  eg size of traps = 3, then iterate a random order like 0, 2, 1, or 2, 0, 1.  
        //then, try to buy the traps and enemies in that order using the generation budget, subtracting the cost of that gameobject.getcomponent<trap>().getCost().  
        //This uses a fisher-yates shuffle.  

        int n = validExtras.Count;
        int[] tempList = new int[n];

        for (int i = 0; i < n; i++) { tempList[i] = i; }

        //Fisher-Yates Shuffle
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = tempList[i];
            tempList[i] = tempList[j];
            tempList[j] = temp;
        }

        //Now that the list is sorted, we effectively choose from the list of valid extras randomly until budget depletes.  
        for (int i = 0; i < n; i++)
        {
            if (generationBudget < minimumCost)
            {
                break; //break if budget is too low to afford any remaining extras
            }
            int index = tempList[i];
            GameObject extra = validExtras[index];
            float cost = extra.GetComponent<GeneratedObject>().getCost();

            if (generationBudget >= cost)
            {
                print("Set " + extra.gameObject.name + " to active");
                extra.SetActive(true);
                generationBudget -= cost;
            }
        }
    }


    public void initializeCell(GameObject left, GameObject right, float budget)
    {
        this.leftCell = left;
        this.rightCell = right;
        this.generationBudget = budget;

        validRightCellCount = validRightCells.Count;

        for (int i = 0; i < validExtras.Count; i++) //figure out, from all the extras in the cell prefab, what the smallest thing that can be bought with the budget is.  
        {
            int currentCost = validExtras[i].GetComponent<GeneratedObject>().getCost(); //If this throws an error, it means you need to add the GeneratedObject script and set it up for the extra.  
            if (minimumCost > currentCost)
            {
                minimumCost = currentCost;
            }
        }
        //since cells are generated by the game manager, we do not worry about the start method, and instead rely on the public initializeCell method to be called by the game manager.  


        //generate traps, enemies, and obstacles for this cell using the budget.  
        generateExtras();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}