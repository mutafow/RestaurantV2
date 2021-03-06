﻿using System.Collections.Generic;
using UnityEngine;

public class Queue : GenericSingletonClass<Queue>
{

	public GameObject customerPrefab;
	[Range(0, 1)]
	public float queueLineSpread = 0.5f;
	[Range(0, 2)]
	public float xDistance = 1;


	private List<Customer> customersInQueue;

	// Executed before the first frame of the game
	private void Start ()
	{
		customersInQueue = new List<Customer>();
		DayCycle.Instance.onMinuteChangedCallback += SpawnCustomers;
	}

	#region add_customer
	public void AddCustomer ( Customer customer )
	{
		GameObject instance = null;
		try
		{
			instance = CustomerManager.Instance.SpawnCustomerModel(customer);
		}
		catch( System.Exception e)
		{
			Debug.LogError(e.Message);
		}
		if ( instance == null ) return;

		customersInQueue.Add(customer);

		Vector3 position = (transform.position + transform.right * (transform.localScale.x / 2f)) - new Vector3(xDistance * customersInQueue.Count, 0, Random.Range(-queueLineSpread, queueLineSpread));
		instance.transform.parent = transform;
		instance.transform.position = position;
		instance.transform.eulerAngles = new Vector3(0, 90 + Random.Range(-30, 30), 0);
	}

	public void AddCustomers ( List<Customer> customers )
	{
		foreach ( Customer customer in customers )
		{
			AddCustomer(customer);
		}
	}
	#endregion

	#region remove_customer
	public void RemoveCustomer ( Customer customer )
	{
		customersInQueue.Remove(customer);

		MoveQueueForwards();
	}

	public void RemoveCustomers ( List<Customer> customers )
	{
		foreach ( var customer in customers )
		{
			RemoveCustomer(customer);
		}
	}
	#endregion

	#region move_customer_to_table
	public void MoveCustomerToTable ( Customer customer, Table table )
	{
		Transform tableTransform = table.transform;

		for ( int i = 0 ; i < tableTransform.childCount ; i++ )
		{
			Transform chair = tableTransform.GetChild(i);

			if ( chair.childCount == 0 )
			{
				GameObject customerGO = GameObject.Find("Customer" + customer.Id);
				if(customerGO == null)
				{
					Debug.LogError("CANT FIND CUSTOMER WITH ID: " + customer.Id);
				}
				customerGO.transform.parent = chair;
				customerGO.transform.localPosition = Vector3.zero;
				customerGO.transform.localEulerAngles = Vector3.zero;
				customerGO.transform.GetChild(0).GetComponent<Animator>().SetBool("isSitting", true);
				CustomerMono customerMono = customerGO.GetComponent<CustomerMono>();
				customerMono.table = table;
				customerMono.OrderFood();
				table.CustomersOnTable++;
				RemoveCustomer(customer);
				break;
			}
		}
	}

	public void MoveCustomersToTable ( List<Customer> customers, Table table )
	{
		foreach ( Customer customer in customers )
		{
			MoveCustomerToTable(customer, table);
		}
	}

	private void MoveQueueForwards ()
	{
		for ( int i = 0 ; i < transform.childCount ; i++ )
		{
			transform.GetChild(i).position += new Vector3(xDistance, 0, 0);
		}
	}
	#endregion

	private void SpawnCustomers ()
	{
		CustomerManager.Instance.GetVisitingNow().ForEach(c => AddCustomers(c.customers));
	}
}
