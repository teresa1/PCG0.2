using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node 
{
    // the position and size of this Leaf
    private const int MIN_LEAF_SIZE = 6;
    public int posX { get; private set; }
    public int posZ { get; private set; }
    public int width { get; private set; }
    public int height { get; private set; }

    bool splitH;
    private int max;
    public Node leftChild; // the Leaf's left child Leaf
    public Node rightChild; // the Leaf's right child Leaf
    public GameObject room; // the room that is inside this Leaf

    private Node helperNode;
    private Node root;
    // public var halls:Vector.; // hallways to connect this Leaf to other Leafs

        
    public Node(int PosX, int PosZ, int Width, int Height)
    {

        posX = PosX;
        posZ = PosZ;
        height = Height;
        width = Width;

    }

    public bool split(List<Node> nodes)
    {
        // begin splitting the leaf into two children
        if (leftChild != null || rightChild != null)
            return false; // we're already split! Abort!

        // determine direction of split
        // if the width is >25% larger than height, we split vertically
        // if the height is >25% larger than the width, we split horizontally
        // otherwise we split randomly


        if (Random.Range(0, 2) > 0.5) {
            splitH = true;
        }

        if (width > height && width/height >= 1.25)
            splitH = false;
        else if (height > width && height/width >= 1.25)
            splitH = true;


        if (splitH) {
            max = height - MIN_LEAF_SIZE;
        }
        else {
            max = width - MIN_LEAF_SIZE;
        }
        if (max <= MIN_LEAF_SIZE)
            return false; // the area is too small to split any more...

        int split = Random.Range(MIN_LEAF_SIZE, max); // determine where we're going to split

        // create our left and right children based on the direction of the split
        if (splitH) {
            //leftChild = new Node(posX, posZ, width, split);
            //rightChild = new Node(posX, posZ + split, width, height - split);
            leftChild = new Node((int)room.transform.localScale.x,(int) room.transform.localScale.y,(int) room.transform.localScale.x, split);
            rightChild = new Node(
                (int)room.transform.position.x,
                (int)room.transform.position.z - (int)((split - room.transform.localScale.z) / 2), (int)room.transform.localScale.x, split );
        }
        else {
            //leftChild = new Node(posX, posZ, split, height);
            //rightChild = new Node(posX + split, posZ, width - split, height);
            leftChild = new Node((int)room.transform.position.x - (int)((split - room.transform.localScale.x) / 2),
                                (int)room.transform.position.z, (int)room.transform.localScale.x, (int)room.transform.localScale.z);

            rightChild = new Node((int)(split - (room.transform.localScale.x) / 2),
               (int) room.transform.position.z, (int)room.transform.localScale.x, (int)room.transform.localScale.z);
        }
        Debug.Log(nodes.Count);
        for (int i = 0; i < nodes.Count; i++) {
            createRooms();
        }
        return true; // split successful!   
    }


   


    public void createRooms()
    {
        // this function generates all the rooms and hallways for this Leaf and all of its children.
        if (leftChild != null || rightChild != null) {
            // this leaf has been split, so go into the children leafs
            if (leftChild != null) {
                leftChild.createRooms();
            }
            if (rightChild != null) {
                rightChild.createRooms();
            }
        }
        else {
            // this Leaf is the ready to make a room
            room = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Vector3 roomSize;
            Vector3 roomPos;
            // the room can be between 3 x 3 tiles to the size of the leaf - 2.
            room.transform.localScale = new Vector3(Random.Range(10, width - 2), Random.Range(10, height - 2));
            // place the room within the Leaf, but don't put it right 
            // against the side of the Leaf (that would merge rooms together)
            room.transform.position = new Vector3(Random.Range(1,room.transform.position.x - (room.transform.localScale.x / 2)), room.transform.position.y,
                                                Random.Range(1, room.transform.position.z - (room.transform.localScale.z/2)));

            //= new Vector3(Random.Range(1, width - room.transform.localScale.x - 1), Random.Range(1, height - room.transform.localScale.z - 1));
            room.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

        }
    }

}

