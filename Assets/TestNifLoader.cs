﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Export;
using Assets.RiftAssets;
using Assets;
using System.IO;
using Assets.Wardrobe;
using Assets.Database;
using Assets.NIF;
using Assets.DatParser;

public class TestNifLoader : MonoBehaviour {
    GameObject mount;
    GameObject test;
    // Use this for initialization
    void Start () {


        this.test = NIFLoader.loadNIFFromFile(@"C:\workspace\guidiff\output\added\a1a6f94a");


        /*
        GameObject character = new GameObject();

        Paperdoll mainPaperdoll = character.AddComponent<Paperdoll>();

        mainPaperdoll.animOverride = "human_male_mount_ape_idle";
        mainPaperdoll.kfbOverride = "human_male_mount.kfb";
        //mainPaperdoll.setKFBPostFix("mount");
        mainPaperdoll.setGender("male");
        mainPaperdoll.setRace("human");
        //mainPaperdoll.GetComponent<AnimatedNif>().animSpeed = 0.001f;
        mainPaperdoll.animSpeed = 0.005f;
        //character.transform.parent = mount.transform;
        character.transform.localPosition = new Vector3(0, 0, 0);
        character.transform.localRotation = Quaternion.identity;
        mainPaperdoll.transform.localRotation = Quaternion.identity;
        mainPaperdoll.setAppearenceSet(1044454339);
        //mainPaperdoll.setGearSlotKey(GearSlot.CAPE, 2131680782);
        //mainPaperdoll.setGearSlotKey(GearSlot.MAIN_HAND, 2131680782);
        //mainPaperdoll.clearGearSlot(GearSlot.HEAD);
        */
    }

    // Update is called once per frame
    void Update () {
        NIFTexturePool.inst.process();
        /*
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F))
        {
            //UnityEditor.PrefabUtility.CreatePrefab("Assets/x.prefab", test);
            if (this.test == null)
                this.test = NIFLoader.loadNIF("world_terrain_5632_5376_split.nif");

        }
#endif
*/
        /*
        if (true)
            return; 
		if (mount == null)
        {

            KFMFile fa = new KFMFile(new FileStream(@"L:\Projects\riftools\RiftTools\build\jar\output\bahmi_female.kfmA", FileMode.Open));
            KFMFile fb = new KFMFile(new FileStream(@"L:\Projects\riftools\RiftTools\build\jar\output\bahmi_female.kfmB", FileMode.Open));

            StreamWriter sw = new StreamWriter("kfma.txt");
            foreach (KFAnimation b in fa.kfanimations.OrderBy(x => x.sequencename))
                sw.WriteLine(b.id + ":" + b.sequencename + ":" + b.sequenceFilename);
            sw.Close();
            sw = new StreamWriter("kfmb.txt");

            foreach (KFAnimation b in fb.kfanimations.OrderBy(x => x.sequencename))
                sw.WriteLine(b.id + ":" + b.sequencename + ":" + b.sequenceFilename);
            sw.Close();

            if (true)
                return;

            DBInst.inst.GetHashCode();

            mount = AnimatedModelLoader.loadNIF(1066487579);
//            mount = AnimatedModelLoader.loadNIF(1823429099);
            AnimatedNif animNif = mount.GetComponent<AnimatedNif>();
            animNif.animSpeed = 0.005f;
            animNif.setSkeletonRoot(mount);
            animNif.setActiveAnimation("mount_haunted_carriage_idle");
            mount.transform.localRotation = Quaternion.identity;
            mount.transform.localPosition = new Vector3(0, -5.91f, 7.66f);

            GameObject character = new GameObject();

            Paperdoll mainPaperdoll = character.AddComponent<Paperdoll>();

            mainPaperdoll.animOverride = "mount_haunted_carriage_idle";
            //mainPaperdoll.kfbOverride = "bahmi_male_mount.kfb";
            mainPaperdoll.setKFBPostFix("mount");
            mainPaperdoll.setGender("male");
            mainPaperdoll.setRace("bahmi");
            //mainPaperdoll.GetComponent<AnimatedNif>().animSpeed = 0.001f;
            mainPaperdoll.animSpeed = 0.005f;
            character.transform.parent = mount.transform;
            character.transform.localPosition = new Vector3(0, 0, 0);
            character.transform.localRotation = Quaternion.identity;
            mainPaperdoll.transform.localRotation = Quaternion.identity;

            if (false)
            {
                mainPaperdoll.FixedUpdate();
                List<KFAnimation> anims = mainPaperdoll.getAnimations();
                Debug.Log("===> anims:" + anims.Count + " ==>" + mainPaperdoll.getState());
                foreach (KFAnimation kf in anims)
                {
                    string s = (kf.sequenceFilename + ":" + kf.sequencename);
                    if (s.Contains("carriage"))
                        Debug.Log(s);
                }
            }

            //mainPaperdoll.updateRaceGender();

            //mainPaperdoll.setAppearenceSet(623293935);
            mainPaperdoll.setAppearenceSet(1044454339);
        }
        */
    }
}
