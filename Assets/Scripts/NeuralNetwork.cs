using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    public Layer[] _layers;
    public int[] _networkShape = {2, 4, 4, 2 };
    public class Layer
    {
        public float[,] _weightsArray;
        public float[] _biasesArray;
        public float[] _nodeArray;
        private int _numberOfInputs;
        private int _numberOfNodes;
        public Layer(int numberOfInputs, int numberOfNodes)
        {
            this._numberOfInputs = numberOfInputs;
            this._numberOfNodes = numberOfNodes;

            _weightsArray = new float[numberOfNodes, numberOfInputs];
            _biasesArray = new float[numberOfNodes];
            _nodeArray = new float[numberOfNodes];
        }

        public void Forward(float[] inputsArray)
        {
            _nodeArray = new float[_numberOfNodes];
            for(int i = 0; i < _numberOfNodes; i++)
            {
                //sum of weights times inputs
                for(int j = 0; j < _numberOfInputs; j++)
                {
                    _nodeArray[i] += _weightsArray[i,j] * inputsArray[j];
                }

                //add the bias
                _nodeArray[i] += _biasesArray[i];
            }
        }

        public void Activation()
        {
            for(int i = 0; i < _numberOfNodes; i++)
            {
                if(_nodeArray[i] < 0)
                {
                    _nodeArray[i] = 0;
                }
            }
        }

        public void MutateLayer(float mutationChance, float mutationAmount)
        {
            for (int i = 0; i < _numberOfNodes; i++)
            {
                for (int j = 0; j < _numberOfInputs; j++)
                {
                    if (Random.value < mutationChance)
                    {
                        _weightsArray[i, j] += Random.Range(-1.0f, 1.0f) * mutationAmount;
                    }
                }

                if (Random.value < mutationChance)
                {
                    _biasesArray[i] += Random.Range(-1.0f, 1.0f) * mutationAmount;
                }
            }
        }

    }
    private void Awake()
    {
        _layers = new Layer[_networkShape.Length - 1];
        for(int i = 0; i < _networkShape.Length; i++)
        {
            _layers[i] = new Layer(_networkShape[i], _networkShape[i + 1]);
        }
    }

    public float[] Brain(float[] inputs)
    {
        for(int i = 0; i < _layers.Length; i++)
        {
            if(i == 0)
            {
                _layers[i].Forward(inputs);
                _layers[i].Activation();
            }
            else if(i == _layers.Length - 1)
                _layers[i].Forward(_layers[i - 1]._nodeArray);
            else
            {
                _layers[i].Forward(_layers[i - 1]._nodeArray);
                _layers[i].Activation();
            }
        }

        return (_layers[_layers.Length - 1]._nodeArray);
    }

    public Layer[] CopyLayers()
    {
        Layer[] tempLayers = new Layer[_networkShape.Length - 1];
        for (int i = 0; i < _layers.Length; i++)
        {
            tempLayers[i] = new Layer(_networkShape[i], _networkShape[i + 1]);
            System.Array.Copy(_layers[i]._weightsArray, tempLayers[i]._weightsArray, _layers[i]._weightsArray.GetLength(0) * _layers[i]._weightsArray.GetLength(1));
            System.Array.Copy(_layers[i]._biasesArray, tempLayers[i]._biasesArray, _layers[i]._biasesArray.GetLength(0));
        }
        return (tempLayers);
    }

    public void MutateNetwork(float mutationChance, float mutationAmount)
    {
        for (int i = 0; i < _layers.Length; i++)
        {
            _layers[i].MutateLayer(mutationChance, mutationAmount);
        }
    }
}
