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

            if (touch.phase == TouchPhase.Began) //TODO: Wyszukiwanie nie powinno odbywaæ siê jako reakcja na tapniêcie 
                                                //- wykrywanie powinno odpaliæ siê w momencie kiedy znajdujemy siê blisko pa³acu
            {
                patternDetector.SearchForObject();
            }
        }
    }
}
