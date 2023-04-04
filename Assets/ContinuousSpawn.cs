using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousSpawn : SpawnArea
{
    // How many items to make per update.
    [SerializeField]
    private int maxPerUpdate = 50;
    
    // How many currently made.
    private int currentlyMade = 0;

    // Start is called before the first frame update.
    void Start()
    {
        
    }

    private void Update()
    {
        if (currentlyMade < madeItems.Length)
            GenerateItems();
    }
    protected override void GenerateItems()
    {
        // Make new items randomly within the collider bounds.
        Bounds b = GetComponent<Collider>().bounds;
        int i = 0;
        while (currentlyMade < madeItems.Length && i < maxPerUpdate)
        {
            i++;
            int index = Random.Range(0, items.Length);
            madeItems[currentlyMade] = MakeItemInArea(b, items[index]);
            currentlyMade++;
        }
    }
}
