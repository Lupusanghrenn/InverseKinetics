using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinetics : MonoBehaviour
{

    public List<Vector3> points;
    public List<float> angles;
    public Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        fabrikAlgo();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;
            fabrikAlgo();
        }
    }

    private void fabrikAlgo(int nbIte=3)
    {
        for(int i = 0; i < nbIte; i++)
        {
            List<Vector3> p = fabrik(points, target);
            p.Reverse();
            points = fabrik(p, points[0]);
            points.Reverse();
        }
        display();
    }

    private List<Vector3> fabrik(List<Vector3> pts,Vector3 target)
    {
        //rappel 1 itérations = 1 aller retour
        //aller
        List<Vector3> newPoints = new List<Vector3>(pts);
        newPoints[newPoints.Count - 1] = target;
        for(int i = newPoints.Count - 2; i >= 0; i--)
        {
            Vector3 direction = pts[i] - newPoints[i + 1];//Vector ab = b-a
            Vector3 directionNOrm = Vector3.Normalize(direction);

            float norme = (pts[i+1] - pts[i]).magnitude;
            newPoints[i] = newPoints[i+1] + norme * directionNOrm;

            //contraintes d angle
            float angleCurrent = Mathf.Rad2Deg * Mathf.Atan2(directionNOrm.y, directionNOrm.x);
            float anglePrevious = 0;
            if (i > 0)
            {
                Vector3 directionPrevious = newPoints[i-1] - newPoints[i];//Vector ab = b-a
                directionPrevious.Normalize();
                anglePrevious = Mathf.Rad2Deg * Mathf.Atan2(directionPrevious.y, directionPrevious.x);
            }

            float angle = angleCurrent - anglePrevious;

            if (angle > angles[i])
            {
                //on bouge les trucs mais samere comment
                Debug.Log("angle" + i);
            }
        }

        return newPoints;

        /*List<Vector3> newPointsRetour = new List<Vector3>(newPoints);
        newPointsRetour[newPointsRetour.Count - 1] = points[0];
        for (int i = 1; i <newPointsRetour.Count; i++)
        {
            Vector3 direction = newPoints[i-1] - newPointsRetour[i];//Vector ab = b-a
            //direction.Normalize();
            Vector3 directionNOrm = Vector3.Normalize(direction);
            float norme = (newPoints[i - 1] - newPoints[i]).magnitude;
            newPointsRetour[i] = norme * directionNOrm;
        }
        points = newPointsRetour;*/
    }

    private void display(string chemin= "Prefabs/Red")
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        foreach(SpriteRenderer s in sprites)
        {
            Destroy(s.gameObject);
        }
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = points.Count;
        GameObject red = (GameObject)Resources.Load(chemin);
        for(int i = 0; i < points.Count; i++)
        {
            lr.SetPosition(i, points[i]);
            Instantiate(red, points[i], Quaternion.identity);
        }

        GameObject targetGO = (GameObject)Resources.Load("Prefabs/Target");
        Instantiate(targetGO, target, Quaternion.identity);

        GameObject start = (GameObject)Resources.Load("Prefabs/Start");
        Instantiate(start, points[0], Quaternion.identity);
    }
}
