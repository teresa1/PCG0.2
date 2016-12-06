using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeStructure : MonoBehaviour
{
    private const int MaxLeafSize = 35;

    const int RiverSize = 1;
    [SerializeField] private int worldWidth = 100;
    [SerializeField] private int worldLenght = 100;

    [SerializeField] private List<Node> nodes;
    [SerializeField] private Node root;

    private Node helperNode;


    void Start()
    {
        CreateNodes();
        CreateRooms(root);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(worldWidth*0.5F, 0.0F, worldLenght * 0.5F), new Vector3(worldWidth, 1.0F, worldLenght));

        //for (int i = 0; i < nodes.Count; i++) {
        //    Gizmos.color = new Color(Random.Range(0.0F, 1.0F), Random.Range(0.0F, 1.0F), Random.Range(0.0F, 1.0F));
        //    Gizmos.DrawCube(new Vector3(nodes[i].PosX + nodes[i].Width*0.5F, nodes[i].PosZ + nodes[i].Height*0.5F, 0.0F), new Vector3(nodes[i].Width, nodes[i].Height, 1.0F));
        //}
    }

    private void CreateNodes()
    {
        root = new Node(0, 0, worldWidth, worldLenght);
        nodes = new List<Node> {root};

        bool didSplit = true;

        // we loop through every Leaf in our Vector over and over again, until no more Leafs can be split.
        while (didSplit) {
            didSplit = false;

            List<Node> copyNodes = new List<Node>(nodes);
            foreach (var node in nodes) {
                if (node.LeftChild == null && node.RightChild == null) // if this Leaf is not already split...
                {
                    // if this Leaf is too big, or 75% chance...
                    if (node.Width > MaxLeafSize || node.Height > MaxLeafSize || Random.Range(0.0F, 1.0F) > 0.25F) {
                        if (node.Split()) // split the Leaf!
                        {
                            // if we did split, push the child leafs to the Vector so we can loop into them next
                            copyNodes.Add(node.LeftChild);
                            copyNodes.Add(node.RightChild);

                            didSplit = true;
                        }
                    }
                }
            }
            nodes = copyNodes;
        }
    }


    private void CreateRooms(Node rootNode)
    {
        // this function generates all the rooms and hallways for this Leaf and all of its children.
        if (rootNode.LeftChild != null || rootNode.RightChild != null) {
            // this leaf has been split, so go into the children leafs
            if (rootNode.LeftChild != null) {
                CreateRooms(rootNode.LeftChild);
            }
            if (rootNode.RightChild != null) {
                CreateRooms(rootNode.RightChild);
            }

            // if there are both left and right children in this Leaf, create a hallway between them
            if (rootNode.LeftChild != null && rootNode.RightChild != null)
            {
              
                    CreateBridges(rootNode.LeftChild.GetRectRoom(), rootNode.RightChild.GetRectRoom(), !rootNode.horizontal);

               
            }
        }
        // this Leaf is the ready to make a room
        else {
            // the room can be between 3 x 3 tiles to the size of the leaf - 2.
            Vector3 roomSize = new Vector3(rootNode.Width - RiverSize, 1.0F, rootNode.Height - RiverSize);
            // place the room within the Leaf, but don't put it right 
            // against the side of the Leaf (that would merge rooms together)
            Vector3 roomPos = new Vector3(rootNode.PosX, 0.0F, rootNode.PosZ);

            GameObject room = GameObject.CreatePrimitive(PrimitiveType.Cube);
            room.transform.position = new Vector3(roomPos.x + roomSize.x*0.5F,  0.0F, roomPos.z + roomSize.z * 0.5F);
            room.transform.localScale = roomSize;
            room.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0F, 1.0F), Random.Range(0.0F, 1.0F), Random.Range(0.0F, 1.0F));

            room.name = "Room";
            room.transform.SetParent(GameObject.Find("Rooms").transform);

            rootNode.SetRoom(room);
        }
    }

    /// <summary>
    ///        now we connect these two rooms together with hallways.
    ///        this looks pretty complicated, but it's just trying to figure out which point is where 
    ///        and then either draw a straight line, or a pair of lines to make a right-angle to connect them.
    ///        you could do some extra logic to make your halls more bendy, or do some more advanced things if you wanted.
    /// </summary>.
    /// <param name="r"></param>
    /// <param name="l"></param>
    public void CreateBridges(Rect r, Rect l, bool horizontal)
    {
        List<GameObject> halls = new List<GameObject>();

        //Vector3 point1 = new Vector3(Random.Range(GetLeft(l) /*+ 1*/, GetRight(l) /*- 2*/), Random.Range(GetBottom(l) /*- 2*/, GetTop(l) /*+ 1*/));
        //Vector3 point2 = new Vector3(Random.Range(GetLeft(r) /*+ 1*/, GetRight(r) /*- 2*/), Random.Range(GetBottom(r) /*- 2*/, GetTop(r) /*+ 1*/));

        Vector3 point1 = new Vector3(l.center.x, 0, l.center.y);
        Vector3 point2 = new Vector3(r.center.x, 0, r.center.y);

        //TODO: DEBUG!
        GameObject rect = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rect.transform.position = new Vector3(point1.x, 1.0F, point1.z);
        rect.transform.localScale = Vector3.one*3;

        GameObject rect2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rect2.transform.position = new Vector3(point2.x,  1.0F, point2.z);
        rect2.transform.localScale = Vector3.one * 3;

        Color color = new Color(Random.value, Random.value, Random.value);
        rect.GetComponent<Renderer>().material.color = color;
        rect2.GetComponent<Renderer>().material.color = color;



        //float w = point2.x - point1.x;
        //float h = point2.y - point1.y;

        if (!horizontal) {
            CreateRectangle(new Vector3(point1.x, 1, (r.yMax + l.yMin) / 2f), new Vector3(1, 0.2f, 1), "v");
        }
        else {
            CreateRectangle(new Vector3((l.xMin + r.xMax)/ 2, 1, point2.x), new Vector3(1, .2f, 1), "h");
        }

        //if (w < 0) {
        //    if (h < 0) {
        //        if (Random.value < 0.5F) {
        //            halls.Add(CreateRectangle(new Vector3(point2.x,point1.y, point1.z), new Vector3(Mathf.Abs(w), 1,1), "h"));
        //            halls.Add(CreateRectangle(new Vector3(point2.x, point2.y, point2.z), new Vector3(1,1, Mathf.Abs(h)), "h"));
        //        }
        //        else {
        //            halls.Add(CreateRectangle(new Vector3(point2.x, point1.y,point2.z), new Vector3(Mathf.Abs(w), 1,1), "h"));
        //            halls.Add(CreateRectangle(new Vector3(point1.x, point1.y , point2.z), new Vector3(1,1, Mathf.Abs(h)), "h"));
        //        }
        //    }
        //    else if (h > 0) {
        //        if (Random.value < 0.5F) {
        //            halls.Add(CreateRectangle(new Vector3(point2.x, point1.y, point1.z), new Vector3(Mathf.Abs(w), 1, 1), "v"));
        //            halls.Add(CreateRectangle(new Vector3(point2.x, point2.y, point1.z), new Vector3(1, 1, Mathf.Abs(h)), "v"));
        //        }
        //        else {
        //            halls.Add(CreateRectangle(new Vector3(point2.x, point1.y, point2.z), new Vector3(Mathf.Abs(w), 1, 1), "h"));
        //            halls.Add(CreateRectangle(new Vector3(point1.x, point2.y, point1.z), new Vector3(1, 1, Mathf.Abs(h)), "h"));
        //        }
        //    }
        //    else // if (h == 0)
        //    {
        //        halls.Add(CreateRectangle(new Vector3(point2.x, point1.y, point2.z), new Vector3(Mathf.Abs(w), 1, 1), "h"));
        //    }
        //}
        //else if (w > 0) {
        //    if (h < 0) {
        //        if (Random.value < 0.5F) {
        //            halls.Add(CreateRectangle(new Vector3(point1.x, point1.y,point2.z), new Vector3(Mathf.Abs(w), 1,1), "v"));
        //                halls.Add(CreateRectangle(new Vector3(point1.x, point1.y, point2.z), new Vector3(1, Mathf.Abs(h)), "v"));
        //            }
        //        else {
        //            halls.Add(CreateRectangle(new Vector3(point1.x, point1.y ,point1.z), new Vector3(Mathf.Abs(w), 1,1), "v"));
        //                halls.Add(CreateRectangle(new Vector3(point2.x,point1.y, point2.z), new Vector3(1, 1, Mathf.Abs(h)), "v"));
        //            }
        //    }
        //    else if (h > 0) {
        //        if (Random.value < 0.5F) {
        //            halls.Add(CreateRectangle(new Vector3(point1.x, point1.y,point1.z), new Vector3(Mathf.Abs(w), 1,1 ), "v"));
        //                halls.Add(CreateRectangle(new Vector3(point2.x, point1.y,point1.z), new Vector3(1, Mathf.Abs(h)), "v"));
        //            }
        //        else {
        //            halls.Add(CreateRectangle(new Vector3(point1.x, point1.y,point2.z), new Vector3(Mathf.Abs(w), 1,1 ), "v"));
        //                halls.Add(CreateRectangle(new Vector3(point1.x,point1.y, point1.z), new Vector3(1,1, Mathf.Abs(h)), "v"));
        //            }
        //    }
        //    else // if (h == 0)
        //    {
        //        halls.Add(CreateRectangle(new Vector3(point1.x, point1.y, point1.z), new Vector3(Mathf.Abs(w), 1,1), "v"));
        //        }
        //}
        //else // if (w == 0)
        //{
        //    if (h < 0) {
        //        halls.Add(CreateRectangle(new Vector3(point2.x, point1.y, point2.z), new Vector3(1,1, Mathf.Abs(h)), "v"));
        //        }
        //    else if (h > 0) {
        //        halls.Add(CreateRectangle(new Vector3(point1.x, point1.y,point1.z), new Vector3(1,1, Mathf.Abs(h)), "v"));
        //    }
        //}
    }

    
    //TODO: Replace GameObject with data model.

    public GameObject CreateRectangle(Vector3 position, Vector3 size, string orientation)
    {
        GameObject rect = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        rect.transform.position = new Vector3(position.x + size.x/2, 1.0F, position.z + size.z / 2);
        rect.transform.position = new Vector3(position.x, 1.0F, position.z );

        rect.transform.localScale = size;

        rect.name = "Bridge" + orientation;
        rect.transform.SetParent(GameObject.Find("Bridges").transform);

        return rect;
    }

    public float GetLeft(GameObject obj)
    {
        GameObject pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointer.transform.position = new Vector3(obj.transform.position.x - obj.transform.localScale.x * 0.5F, pointer.transform.position.y, pointer.transform.position.z);
        pointer.GetComponent<Renderer>().sharedMaterial.color = Color.red;
        pointer.transform.SetParent(GameObject.Find("Pointers").transform);

        return obj.transform.position.x - obj.transform.localScale.x*0.5F /*- obj.transform.localScale.x*0.5F*/;
    }

    public float GetRight(GameObject obj)
    {
        return obj.transform.position.x + obj.transform.localScale.x*0.5F /*- obj.transform.localScale.x*0.5F*/;
    }

    public float GetTop(GameObject obj)
    {
        return obj.transform.position.z + obj.transform.localScale.z*0.5F /*- obj.transform.localScale.y*0.5F*/;
    }

    public float GetBottom(GameObject obj)
    {
        return obj.transform.position.z - obj.transform.localScale.z*0.5F /*- obj.transform.localScale.y*0.5F*/;
    }

}
