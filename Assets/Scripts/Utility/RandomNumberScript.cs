using System;
using System.Security.Cryptography;
using UnityEngine;

//Used to generate random numbers
public class RandomNumberScript : MonoBehaviour
{
	public static int GenerateNumber(int min, int max)
    {
        byte[] byteArray = new byte[2];
        //Using RNGCryptoServiceProvider to fill the byte array with random numbers
        (new RNGCryptoServiceProvider()).GetBytes(byteArray);
        //Applying the modulus operator to the random number so that it's within the specified min and max range, returned as an integer
        return (int)(BitConverter.ToUInt16(byteArray, 0) % max + min);
    }
}