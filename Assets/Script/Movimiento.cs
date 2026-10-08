using UnityEngine;

public class Movimiento : MonoBehaviour
{
    void Start()
    {
        
    }

    public int hp = 100;
    public int DmgPincho = 1;
    public int DmgLava = 1;
    float x;
    float y;
    public int speed = 1;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(x, y, 0) * speed * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.F))
        {
            Vida(DmgPincho);
        }
    }

    public void Vida(int dmg)
    {
        hp -= dmg;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Pincho")
        {
            Vida(DmgPincho);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.transform.tag == "Lava")
        {
            Vida(DmgLava);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Bandera")
        {
            print("WIN");
            Destroy(collision.gameObject);
        }
    }
}
