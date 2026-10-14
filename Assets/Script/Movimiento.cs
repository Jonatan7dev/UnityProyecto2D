using UnityEngine;
using UnityEngine.SceneManagement;

public class Movimiento : MonoBehaviour
{
    public int hp = 100;
    public int DmgPincho = 1;
    public int DmgLava = 1;
    float x;
    float y;
    public int speed = 1;

    public GameObject balaPrefab;
    GameObject balaCopia;


    void Start()
    {
    }



    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(x, y, 0) * speed * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            balaCopia = Instantiate(balaPrefab, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
            
        }
        balaCopia.transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);

       
    }

    public void Vida(int dmg)
    {
        hp -= dmg;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.transform.tag)
        {
            case "Bandera":
                SceneManager.LoadScene("Scene_one");
                break;

            case "Pincho":
                Vida(DmgPincho);
                break;

        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Lava")
        {
            Vida(DmgLava);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bandera")
        {
            print("WIN");
            Destroy(collision.gameObject);
        }
    }
}

