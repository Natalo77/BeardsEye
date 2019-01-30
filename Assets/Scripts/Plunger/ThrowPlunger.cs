using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ThrowPlunger : MonoBehaviour
{
    public Rigidbody2D plunger;
    public float timeBetweenThrows;

    private Animator animator;

    [SerializeField] [Tooltip("'Plunger (enabled)' in Canvas goes here")] private GameObject plungerHUD;
    private Image plungerHUDImage;

    private bool canThrow = true;
    private PlungerHit plungerHitScript;

    [SerializeField] [Tooltip("'Whoosh' Audio Source goes here")] private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Plunger"), LayerMask.NameToLayer("Player"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Plunger"), LayerMask.NameToLayer("Pump"));

        plungerHUDImage = plungerHUD.GetComponent<Image>();
    }

	void Update()
    {
        if (canThrow && Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            if (results.Count == 0)
            {
                ThrowAPlunger();
            }
            else
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].gameObject.tag == "IgnoreRaycast")
                    {
                        break;
                    }
                    else if (i == results.Count - 1)
                    {
                        ThrowAPlunger();
                    }
                }
            }
        }

        if (plungerHUDImage.fillAmount < 1)
        {
            plungerHUDImage.fillAmount += Time.deltaTime / timeBetweenThrows;
        }

        if (UIManager.getGameState() == 0)
        {
            if (plungerHUDImage.fillAmount == 1)
            {
                canThrow = true;
            }
        }
        else if (UIManager.getGameState() == 1)
        {
            canThrow = false;
        }
	}
    

    void ThrowAPlunger()
    {
        animator.SetTrigger("throw");

        Rigidbody2D newPlunger = (Rigidbody2D)Instantiate(plunger, this.transform.position, this.transform.rotation);
        newPlunger.gameObject.layer = LayerMask.NameToLayer("Plunger");

        plungerHitScript = newPlunger.GetComponent<PlungerHit>();

        Vector2 toMouse = getUnitVectorToMouse(newPlunger);
        newPlunger.transform.eulerAngles = new Vector3(newPlunger.transform.eulerAngles.x, newPlunger.transform.eulerAngles.y, 0 + getAngleToMouse(toMouse));

        newPlunger.velocity = toMouse * 30;
        plungerHitScript.directionOfTravel = newPlunger.velocity;

        

        GameOverUIManager.plungersThrown++;
        StartCoroutine(ThrowTimer());
    }

    float getAngleToMouse(Vector2 toMousePos)
    {
        double angle = Mathf.Acos(Vector2.Dot(new Vector2(1,0), toMousePos));

        return (float)angle;
    }

    Vector2 getUnitVectorToMouse(Rigidbody2D newPlunger)
    {
        Camera camera = Camera.main;
        Vector3 mousePos3 = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 startPos = new Vector2(newPlunger.transform.position.x, newPlunger.transform.position.y);
        Vector2 toMousePos = new Vector2(mousePos3.x - startPos.x, mousePos3.y - startPos.y);
        toMousePos.Normalize();

        return toMousePos;
    }

    IEnumerator ThrowTimer()
    {
        canThrow = false;
        float dynamicTimeBetweenThrows = timeBetweenThrows;
        plungerHUDImage.fillAmount = 0;
        audioSource.Play();
        yield return new WaitForSecondsRealtime(timeBetweenThrows);
        canThrow = true;
    }
}