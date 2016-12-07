﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Node
{
    #region Static Fields and Constants

    // the position and size of this Leaf
    private const int MinLeafSize = 20;

    #endregion

    #region Proprieties

    public int PosX { get { return posX; } }
    public int PosZ { get { return posZ; } }
    public int Width { get { return width; } }
    public int Height { get { return height; } }

    /// <summary>
    ///     the Leaf's left child Leaf
    /// </summary>
    public Node LeftChild { get { return leftChild; } }

    /// <summary>
    ///     the Leaf's right child Leaf
    /// </summary>
    public Node RightChild { get { return rightChild; } }

    /// <summary>
    ///     the room that is inside this Leaf
    /// </summary>
    public GameObject Room { get { return room; } }

    #endregion

    #region Fields

    #region Private Fields

    [SerializeField] private Node leftChild;
    [SerializeField] private Node rightChild;
    [SerializeField] private GameObject room;

    [SerializeField] private int posX;
    [SerializeField] private int posZ;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] public bool horizontal;
    #endregion

    #endregion

    #region Constructors

    // public var halls:Vector.; // hallways to connect this Leaf to other Leafs

    public Node(int PosX, int PosZ, int Width, int Height)
    {
        posX = PosX;
        posZ = PosZ;
        height = Height;
        width = Width;
    }

    #endregion

    #region Methods

    #region Public Methods

    // begin splitting the leaf into two children
    public bool Split()
    {
        // we're already split! Abort!
        if (LeftChild != null || RightChild != null)
            return false;

        // determine direction of split
        // if the width is >25% larger than height, we split vertically
        // if the height is >25% larger than the width, we split horizontally
        // otherwise we split randomly

        bool splitH = Random.Range(0.0F, 1.0F) > 0.5F;

        if (Width > Height && Width/Height >= 1.25F)
            splitH = false;
        else if (Height > Width && Height/Width >= 1.25F)
            splitH = true;

        this.horizontal = splitH;

        int max;

        if (splitH) max = Height - MinLeafSize;
        else max = Width - MinLeafSize;

        // the area is too small to split any more...
        if (max <= MinLeafSize)
            return false;

        // determine where we're going to split
        int split = Random.Range(MinLeafSize, max);

        // create our left and right children based on the direction of the split
        if (splitH) {
            leftChild = new Node(PosX, PosZ, Width, split);
            rightChild = new Node(PosX, PosZ + split, Width, Height - split);
        }
        else {
            leftChild = new Node(PosX, PosZ, split, Height);
            rightChild = new Node(PosX + split, PosZ, Width - split, Height);
        }
        // split successful!

        return true;
    }

    public void SetRoom(GameObject room)
    {
        this.room = room;
    }

    /// <summary>
    /// iterate all the way through these leafs to find a room, if one exists.
    /// </summary>
    /// <returns></returns>
    public GameObject GetRoom()
    {
        if (room != null)
            return room;

        GameObject lRoom = null;
        GameObject rRoom = null;

        if (leftChild != null)
            lRoom = leftChild.GetRoom();

        if (rightChild != null)
            rRoom = rightChild.GetRoom();

        if (lRoom == null && rRoom == null)
            return null;

        if (rRoom == null)
            return lRoom;

        if (lRoom == null)
            return rRoom;

        return Random.value > 0.5F ? lRoom : rRoom;
    }


    public Rect GetRectRoom()
    {
        if (room != null) 
            return new Rect(room.transform.position.x - 0.5f * room.transform.localScale.x,
             room.transform.position.z - 0.5f * room.transform.localScale.z,
             room.transform.localScale.x, room.transform.localScale.z);

        // if we do not have a room, there should not be any child missing... 
        Rect lRoom = leftChild.GetRectRoom();
        Rect rRoom = rightChild.GetRectRoom();

        float x1, y1, x2, y2;
        x1 = Mathf.Min(lRoom.xMin, rRoom.xMin);
        x2 = Mathf.Max(lRoom.xMax, rRoom.xMax);
        y1 = Mathf.Min(lRoom.yMin, rRoom.yMin);
        y2 = Mathf.Max(lRoom.yMax, rRoom.yMax);
        return new Rect(x1, y1, x2-x1, y2-y1);


    }

    public void GetBottomRanges(List<Range> r) {
        if (room != null)
        {
            r.Add(new Range(room.transform.position.x - 0.5f * room.transform.localScale.x,
                            room.transform.position.x + 0.5f * room.transform.localScale.x));
        }
        else
        {
            if (!horizontal) 
                leftChild.GetBottomRanges(r);
            
            rightChild.GetBottomRanges(r);
        }
    }
    public void GetTopRanges(List<Range> r) {
        if (room != null)
        {
            r.Add(new Range(room.transform.position.x - 0.5f * room.transform.localScale.x,
            room.transform.position.x + 0.5f * room.transform.localScale.x));
        }
        else
        {
            if (!horizontal)
                rightChild.GetTopRanges(r);
            
            leftChild.GetTopRanges(r);
        }
    }
      public void GetLeftRanges(List<Range> r) {
        if (room != null)
        {
            r.Add(new Range(room.transform.position.z - 0.5f * room.transform.localScale.z,
            room.transform.position.z + 0.5f * room.transform.localScale.z));
        }
        else
        {
            if (horizontal)
                rightChild.GetLeftRanges(r);
            
            leftChild.GetLeftRanges(r);
        }
    }
        public void GetRightRanges(List<Range> r) {
        if (room != null)
        {
            r.Add(new Range(room.transform.position.z - 0.5f * room.transform.localScale.z,
            room.transform.position.z + 0.5f * room.transform.localScale.z));
        }
        else
        {
            if (horizontal)
                leftChild.GetRightRanges(r);
            
            rightChild.GetRightRanges(r);
        }
    }
   
    #endregion

    #endregion
}