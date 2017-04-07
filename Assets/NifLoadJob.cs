﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.NIF;
using System.Threading;

public class NifLoadJob : ThreadedJob
{
    public volatile static int count = 0;

    static Dictionary<String, GameObject> originals = new Dictionary<string, GameObject>();
    static Dictionary<String, Semaphore> cacheWait = new Dictionary<string, Semaphore>();

    public static GameObject getCachedObject(string fn)
    {
        lock (originals)
        {
            if (originals.ContainsKey(fn))
            {
                GameObject go = originals[fn];
                GameObject newG = GameObject.Instantiate(go);
                return newG;
            }
            return null;
        }
    }


    //public Vector3[] InData;  // arbitary job data
    //public Vector3[] OutData; // arbitary job data
    public string filename;

    public telara_obj parent;
    NIFLoader loader;
    NIFFile niffile;


    public NifLoadJob(NIFLoader loader, string file)
    {
        this.loader = loader;
        this.filename = file;
        lock (cacheWait)
        {
            if (!cacheWait.ContainsKey(filename))
                cacheWait[filename] = new Semaphore(1, 1);
        }
    }
    protected override void ThreadFunction()
    {
        // Do your threaded task. DON'T use the Unity API here

        count++;

        cacheWait[filename].WaitOne();

        lock (originals)
        {
            // if our cache contains an object, return it immediately
            if (originals.ContainsKey(filename))
                return;
        }
        try
        {
            niffile = loader.getNIF(filename);
        }
        catch (Exception ex)
        {
            Debug.Log("there was an exception while doing the thread:" + filename + ": " + ex);
        }
    }
    protected override void OnFinished()
    {
        try
        {
            GameObject go;

            count--;
            // This is executed by the Unity main thread when the job is finished
            if (niffile != null)
            {
                go = loader.loadNIF(niffile, filename);
                lock (originals)
                {
                    originals[filename] = go;
                }
            }
            else
                go = getCachedObject(filename);

            if (go != null)
            {
                go.transform.SetParent(parent.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Unable to load nif:" + niffile + " " + filename);
        }
        finally
        {
            cacheWait[filename].Release();
        }
    }
}