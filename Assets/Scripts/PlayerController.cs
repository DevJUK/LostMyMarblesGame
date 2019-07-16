﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	public float MoveSpeed = 20;

	[Range(0,50)]
	public float JumpHeight;
	public float FallSpeed;

	[Range(0, 15)]
	public float SpeedFalloff = 15;
	public float SpeedReductionRate;
	public float FallOffDelay;

	public bool UseCameraPoint = true;
	public bool CanJump = true;

	private GameObject MoveDirGO;

	private Vector3 StartPos;

	private bool IsFalloffRunning;

	private EndPadScript EPScript;

	private bool LevelFailed;

	private void Start()
	{
		if (UseCameraPoint)
		{
			MoveDirGO = GameObject.FindGameObjectWithTag("CameraPoint");
		}
		HideMouse();
		EPScript = FindObjectOfType<EndPadScript>();
	}


	private void Update()
	{
		if ((Input.GetButtonDown("Jump") && (CanJump)))
		{
			Debug.Log("Jump Pressed");
			GetComponent<Rigidbody>().velocity += Vector3.up * JumpHeight;
		}

		Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if (UseCameraPoint)
		{
			Movement = MoveDirGO.transform.TransformDirection(Movement);
		}

		GetComponent<Rigidbody>().AddForce(Movement * MoveSpeed);


		JumpSmoothing();

		if (LevelFailed)
		{
			ResetScene();
		}
	}


	private void JumpSmoothing()
	{
		//if (GetComponent<Rigidbody>().velocity.y < 0)
		//{
			//GetComponent<Rigidbody>().velocity += Vector3.up * Physics.gravity.y * (FallSpeed - 1) * Time.deltaTime;

		GetComponent<Rigidbody>().AddForce(Vector3.up * Physics.gravity.y * (FallSpeed - 1) * Time.deltaTime, ForceMode.Impulse);
		//}
		//else if (GetComponent<Rigidbody>().velocity.y > 0 && !Input.GetButton("Jump"))
		//{
		//	GetComponent<Rigidbody>().velocity += Vector3.up * Physics.gravity.y * (JumpHeight - 1) * Time.deltaTime;
		//}
	}


	// Just toggles whether the mouse is visible in the game or not
	internal void HideMouse()
	{
		if (Cursor.lockState == CursorLockMode.None) { Cursor.lockState = CursorLockMode.Locked; }
		else { Cursor.lockState = CursorLockMode.None; }
		Cursor.visible = !Cursor.visible;
	}


	// Resets the game when the player dies
	private void ResetScene()
	{
		if (Input.GetMouseButtonDown(0))
		{
			GameObject.FindGameObjectWithTag("OutOfBounds").GetComponentInChildren<Animator>().SetBool("IsOFB", false);
			string ThisScene = SceneManager.GetActiveScene().name;
			Debug.Log(ThisScene);
			SceneManager.LoadSceneAsync(ThisScene);
			LevelFailed = false;
		}
		else
		{
			GameObject.FindGameObjectWithTag("OutOfBounds").GetComponentInChildren<Animator>().SetBool("IsOFB", true);
		}
	}


	private void OnTriggerEnter(Collider other)
	{
		// Does all the out of bounds stuff
		switch (other.gameObject.tag)
		{
			case "OutOfBounds":
				Debug.Log("Out Of Bounds");
				other.gameObject.GetComponentInChildren<Canvas>().enabled = true;
				Camera.main.transform.parent.LookAt(gameObject.transform);
				Camera.main.GetComponentInParent<CameraController>().enabled = false;
				HideMouse();
				LevelFailed = true;
				break;
			case "Gem":
				Debug.Log("Gem Collected");
				other.gameObject.SetActive(false);
				EPScript.GemsCollected++;
				break;
			case "Zomball":
				Debug.Log("Zomball Hit Player");
				GameObject.FindGameObjectWithTag("OutOfBounds").GetComponentInChildren<Text>().text = "A Zom-ball Ate You!";
				GameObject.FindGameObjectWithTag("OutOfBounds").GetComponentInChildren<Canvas>().enabled = true;
				Camera.main.transform.parent.LookAt(gameObject.transform);
				Camera.main.GetComponentInParent<CameraController>().enabled = false;
				HideMouse();
				LevelFailed = true;
				break;
			default:
				break;
		}
	}
}
