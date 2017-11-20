﻿using System;
using GrassSimulation.Grass;
using UnityEngine;
using Random = System.Random;

namespace GrassSimulation
{
	[Serializable]
	public class SimulationContext : IInitializable
	{
		public Camera Camera;
		public EditorSettings EditorSettings;
		public ComputeShader GrassSimulationComputeShader;
		public Material GrassMaterial;
		public Texture2D Heightmap;
		[HideInInspector] public bool IsReady;
		[HideInInspector] public int KernelCulling;
		[HideInInspector] public int KernelPhysics;
		[HideInInspector] public int KernelSimulationSetup;
		[HideInInspector] public Random Random;
		public SimulationSettings Settings;
		public SharedGrassData SharedGrassData;
		public Terrain Terrain;
		public Transform Transform;

		public bool Init()
		{
			if (!Camera || !Terrain || !Transform || !GrassSimulationComputeShader)
			{
				Debug.LogWarning("GrassSimulation: Not all dependencies are set.");
				if (!Camera) Debug.Log("GrassSimulation: Camera not set.");
				if (!Terrain) Debug.Log("GrassSimulation: Terrain not set.");
				if (!Transform) Debug.Log("GrassSimulation: Transform not set.");
				if (!GrassSimulationComputeShader) Debug.Log("GrassSimulation: GrassSimulationComputeShader not set.");
				IsReady = false;
				return false;
			}

			if (Settings == null)
			{
				Settings = new SimulationSettings();
				//Settings.GrassBlade = Texture2D.whiteTexture;
			}
			if (EditorSettings == null) EditorSettings = new EditorSettings();

			//Build Heightmap Texture
			Heightmap = Utils.Terrain.CreateHeightmapFromTerrain(Terrain);

			//Create a single random object
			Random = new Random(Settings.RandomSeed);

			//Find kernels for ComputeShaders
			KernelPhysics = GrassSimulationComputeShader.FindKernel("PhysicsMain");
			KernelCulling = GrassSimulationComputeShader.FindKernel("CullingMain");
			KernelSimulationSetup =  GrassSimulationComputeShader.FindKernel("SimulationSetup"); 

			//Create sharedGrassData
			SharedGrassData = new SharedGrassData(this);
			SharedGrassData.Init();
			
			GrassMaterial.SetTexture("GrassBlade", Settings.GrassBlade);
			GrassSimulationComputeShader.SetFloat("LodDistanceFullDetail", Settings.LodDistanceFullDetail);
			GrassSimulationComputeShader.SetFloat("LodDistanceBillboard", Settings.LodDistanceBillboard);
			GrassSimulationComputeShader.SetFloat("LodDistanceMax", Settings.LodDistanceMax);
			GrassSimulationComputeShader.SetFloat("LodDensityFullDetailDistance", Settings.LodDensityFullDetailDistance);
			GrassSimulationComputeShader.SetFloat("LodDensityBillboardDistance", Settings.LodDensityBillboardDistance);
			GrassSimulationComputeShader.SetFloat("LodDensityMaxDistance", Settings.LodDensityMaxDistance);

			//Everything is ready.
			IsReady = true;
			return true;
		}
	}
}