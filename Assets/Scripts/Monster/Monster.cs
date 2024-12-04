using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public bool _mutateMutations = true;
    public GameObject _monsterPrefab;
    public bool _isUser = false;
    public bool _canEat = true;
    public float _viewDistance = 20;
    public float _size = 1.0f;
    public float _energy = 20;
    public float _energyGained = 10;
    public float _reproductionEnergyGained = 1;
    public float _reproductionEnergy = 0;
    public float _reproductionEnergyThreshold = 2;
    public float _FB = 0;
    public float _LR = 0;
    public int _numberOfChildren = 1;
    private bool _isMutated = false;
    float _elapsed = 0f;
    public float _lifeSpan = 0f;
    public float[] _distances = new float[6];

    public float _mutationAmount = 0.8f;
    public float _mutationChance = 0.2f;
    public NeuralNetwork _nn;
    public MonsterMovement _movement;

    float relativeFoodX;
    float relativeFoodZ;

    private List<GameObject> edibleFoodList = new List<GameObject>();

    public bool isDead = false;

    void Awake()
    {
        _nn = gameObject.GetComponent<NeuralNetwork>();
        _movement = gameObject.GetComponent<MonsterMovement>();
        _distances = new float[6];

        this.name = "Monster";
    }
    void FixedUpdate()
    {
        
        if (!_isMutated)
        {
            //call mutate function to mutate the neural network
            MutateCreature();
            this.transform.localScale = new Vector3(_size, _size, _size);
            _isMutated = true;
            _energy = 20;
        }

        ManageEnergy();
        #region Raycast
        int numberRaycasts = 5;
        float angleBetweenRaycasts = 30;
        RaycastHit hit;
        for (int i = 0; i < numberRaycasts; i++)
        {
            float angle = ((2 * i + 1 - numberRaycasts) * angleBetweenRaycasts / 2);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 rayDirection = rotation * transform.forward * -1;
            Vector3 rayStart = transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(rayStart, rayDirection, out hit, _viewDistance))
            {
                Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.red);
                if (hit.transform.gameObject.tag == "Food")
                {
                    //use the length of the raycast as the distance to the food object
                    _distances[i] = hit.distance / _viewDistance;
                }
                else //if no food object is detected, set the distance to the maximum length of the raycast
                    _distances[i] = 1;
            }
            else
            {
                Debug.DrawRay(rayStart, rayDirection * _viewDistance, Color.red);
                _distances[i] = 1;
            }
        }
        #endregion

        #region NeuralNetwork
        //setup inputs for the neural network
        float[] inputsToNN = _distances;

        //get outputs from the neural network
        float[] outputsFromNN = _nn.Brain(inputsToNN);

        //store the outputs from the neural network in variables
        _FB = outputsFromNN[0];
        _LR = outputsFromNN[1];

        _movement.Move(_FB, _LR);
        #endregion
    }

    #region EatFood
    //eat food
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Food" && _canEat)
        {
            GameObject obj = col.gameObject;
            _energy += _energyGained;
            _reproductionEnergy += _reproductionEnergyGained;
            //Destroy(col.gameObject);
            obj.SetActive(false);
        }
    }
    #endregion

    public void ManageEnergy()
    {
        _elapsed += Time.deltaTime;
        _lifeSpan += Time.deltaTime;
        if (_elapsed >= 1f) //subtract 1 energy per second
        {
            _elapsed = _elapsed % 1f;
            _energy -= 1f;

            //if monster has enough energy to reproduce, reproduce
            if (_reproductionEnergy >= _reproductionEnergyThreshold)
            {
                _reproductionEnergy = 0;
                Reproduce();
            }
        }

        //if fall then die, starve then die
        float monsterY = this.transform.position.y;
        if (_energy <= 0 || monsterY < -10)
        {
            this.transform.Rotate(0, 0, 180);
            Destroy(this.gameObject, 3);
            GetComponent<MonsterMovement>().enabled = false;
        }
    }

    private void MutateCreature()
    {
        if (_mutateMutations)
        {
            _mutationAmount += Random.Range(-1.0f, 1.0f) / 100;
            _mutationChance += Random.Range(-1.0f, 1.0f) / 100;
        }

        //make sure mutation amount and chance are positive using max function
        _mutationAmount = Mathf.Max(_mutationAmount, 0);
        _mutationChance = Mathf.Max(_mutationChance, 0);

        _nn.MutateNetwork(_mutationAmount, _mutationChance);
    }
    GameObject FindClosestFood()
    {
        GameObject closestFood = null;
        float monsterX;
        float monsterZ;
        float foodX = 0;
        float foodZ = 0;

        float minFoodDistance = -1;
        float foodDistance = 0;
        int minFoodIndex = -1;
        bool foodInRange = false;

        monsterX = this.transform.position.x;
        monsterZ = this.transform.position.z;

        //use a sphere cast to find all food in range by viewDistance of the agent and add them to a list of edible food.
        if (Random.value * 100 < 5)
        {
            edibleFoodList.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, _viewDistance);
            foreach (var hit in hitColliders)
            {
                if (hit.gameObject.tag == "Food")
                {
                    edibleFoodList.Add(hit.gameObject);
                }
            }
        }

        //find closest food in range of monster
        if (Random.value * 100 < 50)
        {
            for (int i = 0; i < edibleFoodList.Count; i++)
            {
                if (edibleFoodList[i] != null)
                {
                    foodX = edibleFoodList[i].transform.position.x;
                    foodZ = edibleFoodList[i].transform.position.z;

                    foodDistance = Mathf.Sqrt((Mathf.Pow(foodX - monsterX, 2) + Mathf.Pow(foodZ - monsterZ, 2)));
                    if (foodDistance < minFoodDistance || minFoodDistance < 0)
                    {
                        minFoodDistance = foodDistance;
                        minFoodIndex = i;
                        if (minFoodDistance < _viewDistance)
                        {
                            closestFood = edibleFoodList[i];
                            foodInRange = true;
                        }
                    }
                }
            }
        }

        return (closestFood);
    }

    public void Reproduce()
    {
        for (int i = 0; i < _numberOfChildren; i++) //create how many children at a time
        {
            //create a new agent and set its position to the parent's position and random offset in the x and z directions 
            GameObject child = Instantiate(_monsterPrefab, new Vector3((float)this.transform.position.x + Random.Range(-10, 11), 0.75f, (float)this.transform.position.z + Random.Range(-10, 11)), Quaternion.identity);

            //copy the parent's neural network to the child
            child.GetComponent<NeuralNetwork>()._layers = GetComponent<NeuralNetwork>().CopyLayers();
        }
        _reproductionEnergy = 0;

    }
}