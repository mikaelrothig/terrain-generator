using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject Hexagon;
    public GameObject HexParentHolder;
    public int heightMultiplier = 2;
    public float Scale = 15;
    public float seed;

    public int mapSize = 5;
    float hexRadius = 1f;

    public float WaterLevel = 1;
    public float SandUpToThisHeight = 1.2f;
    public float GrassUpToThisHeight = 1.6f;

    public bool Randomize;
    public bool UpdateMesh;

    public Material Water;
    public Material Sand;
    public Material Grass;
    public Material Stone;
    public Material Frost;

    public GameObject[] trees;
    public GameObject[] rocks;

    void Start()
    {
        GenerateMesh();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Randomize = true;
            UpdateMesh = true;
        }

        if (UpdateMesh != false)
        {
            foreach (Transform child in HexParentHolder.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
            foreach (GameObject tree in trees)
            {
                Destroy(tree);
            }

            GenerateMesh();
            UpdateMesh = false;
        }
    }

    public void GenerateMesh()
    {
        if (Randomize)
        {
            seed = Random.Range(0, 1000000);
            Scale = Random.Range(10, 20);
            WaterLevel = Random.Range(0.8f, 2f);
            heightMultiplier = Mathf.RoundToInt(WaterLevel) + Random.Range(1, 6);
            SandUpToThisHeight = WaterLevel + heightMultiplier / 5 + Random.Range(0.2f, 0.6f);
            GrassUpToThisHeight = (WaterLevel / 2) + SandUpToThisHeight + Random.Range(0.2f, 1f);
        }

        Vector3 pos = Vector3.zero;

        for (int q = -mapSize; q <= mapSize; q++)
        {
            int r1 = Mathf.Max(-mapSize, -q - mapSize);
            int r2 = Mathf.Min(mapSize, -q + mapSize);
            for (int r = r1; r <= r2; r++)
            {

                pos.x = hexRadius * 3.0f / 2.0f * q;
                pos.z = hexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);

                CaculateHeights(pos.x, pos.z);
            }
        }

        //Instantiate  Waterlevel
        var water = Instantiate(Hexagon, new Vector3(0, WaterLevel - 1.75f, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
        float waterScale = mapSize * 1.75f;
        water.transform.localScale = new Vector3(waterScale, waterScale, 1.65f);
        water.GetComponent<MeshRenderer>().material = Water;
        water.transform.parent = HexParentHolder.transform;

        var frost = Instantiate(Hexagon, new Vector3(0, WaterLevel, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
        frost.transform.localScale = new Vector3(waterScale, waterScale, .1f);
        frost.GetComponent<MeshRenderer>().material = Frost;
        frost.transform.parent = HexParentHolder.transform;
    }

    void CaculateHeights(float x, float z)
    {
        float newX = x;
        float newZ = z;

        float xCord = newX / Scale + seed;
        float zCord = newZ / Scale + seed;

        //generate height based on noise
        float roundedHeight = Mathf.PerlinNoise(xCord, zCord);

        //spawn Hex

        var Hex = Instantiate(Hexagon, new Vector3(newX, 0, newZ), Quaternion.Euler(new Vector3(90, 30, Hexagon.transform.rotation.z)));
        Hex.name = x + "," + z;

        roundedHeight *= heightMultiplier;

        //set the height to double because it goes both up and down
        Hex.transform.localScale = new Vector3(Hex.transform.localScale.x, Hex.transform.localScale.y, roundedHeight * 2);

        Hex.transform.parent = HexParentHolder.transform;
        SetHexType(Hex, roundedHeight);
    }

    void SetHexType(GameObject Hex, float HexHeight)
    {
        if (HexHeight <= WaterLevel)
        {
            //water
            //Hex.GetComponent<MeshRenderer>().material = Water;
            Destroy(Hex);

            //set the height to water height
            //Hex.transform.localScale = new Vector3(Hex.transform.localScale.x, Hex.transform.localScale.y, WaterLevel * 2);
            //Hex.transform.localScale = new Vector3(Hex.transform.localScale.x, Hex.transform.localScale.y, Hex.transform.localScale.z);

            //set the position back to zero
            //Hex.transform.position = new Vector3(Hex.transform.position.x, 0, Hex.transform.position.z);
        }

        if (HexHeight > WaterLevel)
        {
            if (HexHeight <= SandUpToThisHeight)
            {
                //sand
                Hex.GetComponent<MeshRenderer>().material = Sand;
                //Hex.tag = ("Sand");
            }
        }

        if (HexHeight > SandUpToThisHeight)
        {
            if (HexHeight < GrassUpToThisHeight)
            {
                //grass
                Hex.GetComponent<MeshRenderer>().material = Grass;
                //Hex.tag = ("Grass");

                if (Random.Range(0f, 5f) > 3f && HexHeight > WaterLevel)
                {
                    Vector3 pos = new Vector3(Hex.transform.position.x, Hex.transform.localScale.z + 1, Hex.transform.position.z);
                    GameObject tree;
                    /*GameObject tree = Instantiate(AcaciaTree, pos, Quaternion.Euler(new Vector3(0,Random.Range(0f, 360f),0)));*/
                    if (Random.Range(1, 3) == 1)
                        tree = Instantiate(trees[0], pos, Quaternion.Euler(new Vector3(-90, Random.Range(0f, 360f), 0)));
                    else if (Random.Range(1, 3) == 2)
                        tree = Instantiate(trees[1], pos, Quaternion.Euler(new Vector3(-90, Random.Range(0f, 360f), 0)));
                    else
                        tree = Instantiate(trees[2], pos, Quaternion.Euler(new Vector3(-90, Random.Range(0f, 360f), 0)));

                    tree.tag = "Tree";
                }
            }
        }

        if (HexHeight > GrassUpToThisHeight)
        {
            //rock
            Hex.GetComponent<MeshRenderer>().material = Stone;
            //Hex.tag = ("Stone");

            /*if (Random.Range(0f, 5f) > 3f && HexHeight > WaterLevel)
                {
                    if (Random.Range(1, 3) == 1)
                        rock = Instantiate(rocks[0], new Vector3(pos.x, pos.y-0.5f, pos.z), Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0)));
                    else if (Random.Range(1, 3) == 2)
                        rock = Instantiate(rocks[1], new Vector3(pos.x, pos.y-0.5f, pos.z), Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0)));
                    else
                        rock = Instantiate(rocks[2], new Vector3(pos.x, pos.y-0.5f, pos.z), Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0)));

                    //rock.tag = "Rock";
                }*/
        }
    }
}
