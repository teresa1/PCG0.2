using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    // the position and size of this Leaf
    private const int MIN_LEAF_SIZE = 6;
    private const int MAX_LEAF_SIZE = 20;
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


    private List<Node> nodes;
    public Node(int PosX, int PosZ, int Width, int Height)
    {
       

        posX = PosX;
        posZ = PosZ;
        height = Height;
        width = Width;

        
        CreatNodes();
        split();
        createRooms();


    }

    public bool split()
    {
        root = new Node(0, 0, width, height);
        // begin splitting the leaf into two children
        if (leftChild != null || rightChild != null)
            return false; // we're already split! Abort!

        // determine direction of split
        // if the width is >25% larger than height, we split vertically
        // if the height is >25% larger than the width, we split horizontally
        // otherwise we split randomly


        if (Random.RandomRange(0, 2) > 0.5) {
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
            leftChild = new Node(posX, posZ, width, split);
            rightChild = new Node(posX, posZ + split, width, height - split);
        }
        else {
            leftChild = new Node(posX, posZ, split, height);
            rightChild = new Node(posX + split, posZ, width - split, height);
        }

        for (int i = 0; i < nodes.Count; i++) {
            root.createRooms();
        }
        return true; // split successful!
       
        
    }


    void CreatNodes()
    {
        nodes = new List<Node>();

        nodes.Add(root);

        bool did_split = true;
        // we loop through every Leaf in our Vector over and over again, until no more Leafs can be split.
        while (did_split) {
            did_split = false;
            for (int i = 0; i < nodes.Count; i++) {
                if (helperNode.leftChild == null && helperNode.rightChild == null) // if this Leaf is not already split...
                {
                    // if this Leaf is too big, or 75% chance...
                    if (helperNode.width > MAX_LEAF_SIZE || helperNode.height > MAX_LEAF_SIZE || Random.Range(0,2)> 0.25) {
                        if (helperNode.split()) // split the Leaf!
                        {
                            // if we did split, push the child leafs to the Vector so we can loop into them next
                            nodes.Add(helperNode.leftChild);
                            nodes.Add(helperNode.rightChild);
                            did_split = true;
                        }
                    }
                }
            }
        }
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
            Vector3 roomSize;
            Vector3 roomPos;
            // the room can be between 3 x 3 tiles to the size of the leaf - 2.
            roomSize = new Vector3(Random.Range(3, width - 2), Random.Range(3, height - 2));
            // place the room within the Leaf, but don't put it right 
            // against the side of the Leaf (that would merge rooms together)
            roomPos = new Vector3(Random.Range(1, width - roomSize.x - 1), Random.Range(1, height - roomSize.y - 1));

            room = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
    }




}

