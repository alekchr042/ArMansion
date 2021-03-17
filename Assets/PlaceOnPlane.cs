using UnityEngine;

public class PlaceOnPlane : MonoBehaviour
{
    private PatternDetector patternDetector;

    // Start is called before the first frame update
    void Start()
    {
        patternDetector = GetComponent<PatternDetector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) //TODO: Wyszukiwanie nie powinno odbywa� si� jako reakcja na tapni�cie 
                                                //- wykrywanie powinno odpali� si� w momencie kiedy znajdujemy si� blisko pa�acu
            {
                patternDetector.SearchForObject();
            }
        }
    }
}
