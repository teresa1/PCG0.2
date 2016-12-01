using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeStructure : MonoBehaviour
{

    private const int MAX_LEAF_SIZE = 20;

    private List<Node> nodes;
    private Node root;
    private Node helperNode;


    void Start ()
    {
	//Node node  = new Node(0,0,100,100);
        CreatNodes();
    //node.split(nodes);

	}


    void CreatNodes()
    {
        nodes = new List<Node>();
       
        root = new Node(0,0,100,100);
        nodes.Add(root);



        bool did_split = true;
        // we loop through every Leaf in our Vector over and over again, until no more Leafs can be split.
        while (did_split)
        {
            did_split = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (root.leftChild == null && root.rightChild == null) // if this Leaf is not already split...
                {
                    helperNode = nodes[i];
                    // if this Leaf is too big, or 75% chance...
                    if (helperNode.width > MAX_LEAF_SIZE || helperNode.height > MAX_LEAF_SIZE || Random.Range(0, 2) > 0.25)
                    {
                        if (helperNode.split(nodes)) // split the Leaf!
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

    void splitNodes()
    {
        for (int i = 0; i < nodes.Count; i++) {
            nodes[i].split(nodes);
        }
    }

}
