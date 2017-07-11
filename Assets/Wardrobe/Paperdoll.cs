﻿using Assets.Database;
using Assets.DatParser;
using Assets.NIF;
using Assets.RiftAssets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Wardrobe
{
    public delegate void OnFinished();
    public delegate void ThreadedCall();

    public class Paperdoll : MonoBehaviour
    {
        public enum ClassState
        {
            DB_LOADING,
            IDLE,
            UPDATE,
        }
        ClassState state = ClassState.DB_LOADING;

        GameObject refModel;
        GameObject costumeParts;
        AnimatedNif animationNif;
        string raceString = "human";
        string genderString = "male";
        public float animSpeed = 0.01f;
        Dictionary<GearSlot, GameObject> gearSlotObjects = new Dictionary<GearSlot, GameObject>();
        Dictionary<GearSlot, long> gearSlotKeys = new Dictionary<GearSlot, long>();
        long appearenceSet = long.MinValue;

        public Paperdoll() : base()
        {
            DBInst.loadOrCallback((d) => state = ClassState.UPDATE);
        }
        /// <summary>
        /// Get animations, will block if not ready
        /// </summary>
        /// <returns></returns>
        public List<KFAnimation> getAnimations()
        {
            checkState();
            //while (state != ClassState.IDLE) ;
            if (animationNif != null)
                return animationNif.getAnimations();
            return new List<KFAnimation>();
        }

        private void checkState()
        {
            if (DBInst.loaded && state == ClassState.DB_LOADING)
                Debug.LogError("db is loaded but we still think it's not?");
        }
        
        public string getGenderString()
        {
            return genderString;
        }
        public string getRaceString()
        {
            return raceString;
        }
        void Start()
        {
            
        }

        string getBaseModel()
        {
            return string.Format("{0}_{1}", raceString, genderString);
        }

        public string kfbOverride = "";
        public string animOverride = "";

        public void clearAppearence()
        {
            this.appearenceSet = long.MinValue;

            if (state == ClassState.IDLE)
                state = ClassState.UPDATE;
        }
        private void checkFemaleModesty()
        {
            // for some reason all female models except bahmi are "nude"
            // so if the model is female, give it some clothes for "modesty" because some people/cultures may be offended
            if (genderString.Equals("female"))
            {
                if (!gearSlotKeys.ContainsKey(GearSlot.TORSO))
                    setGearSlotKey(GearSlot.TORSO, 1127855431);
                if (!gearSlotKeys.ContainsKey(GearSlot.LEGS))
                    setGearSlotKey(GearSlot.LEGS, 1300181064);
            }
        }
        private void updateRaceGender()
        {
            if (state != ClassState.UPDATE)
            {
                Debug.LogError("Cannot update race/gender without being update mode");
                return;
            }


            if (refModel != null)
                GameObject.Destroy(refModel);
            if (costumeParts != null)
                GameObject.Destroy(costumeParts);

            // defines the base model
            string nif = string.Format("{0}_refbare.nif", getBaseModel());
            string kfm = string.Format("{0}.kfm", getBaseModel());
            string kfb = string.Format("{0}.kfb", getBaseModel());
            if (!"".Equals(kfbOverride))
                kfb = kfbOverride;

            
            animationNif = this.gameObject.GetComponent<AnimatedNif>();
            if (animationNif == null)
                animationNif= this.gameObject.AddComponent<AnimatedNif>();
            animationNif.setParams(AssetDatabaseInst.DB, nif, kfm, kfb);

            NIFFile file = NIFLoader.getNIF(nif);
            GameObject go = NIFLoader.loadNIF(file, nif, true);
            go.transform.parent = this.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            refModel = go;


            animationNif.setActiveAnimation(string.Format("{0}_unarmed_idle", getBaseModel()));
            if (!"".Equals(animOverride))
                animationNif.setActiveAnimation(animOverride);
            animationNif.setSkeletonRoot(refModel);
                
            costumeParts = new GameObject("CostumeParts");
            costumeParts.transform.parent = refModel.transform;

            // always hide the boots
            enableDisableGeo("boots", go, false);

        }

   


        public void setGearSlotKey(GearSlot slot, long key)
        {
            //Debug.Log("set gear slot[" + slot + "] to  key " + key);
            gearSlotKeys[slot] = key;
            if (state == ClassState.IDLE)
                state = ClassState.UPDATE;
        }

        private void updateGearSlotObject(GearSlot slot)
        {
            //Debug.Log("update gear slot [" + slot + "]");
            if (state != ClassState.UPDATE)
            {
                Debug.LogError("Cannot update gear slot[" + slot + "] without being update mode");
                return;
            }

            // destroy any existing gear slot
            //Debug.Log("try destroy update gear slot object");
            if (gearSlotObjects.ContainsKey(slot))
                GameObject.Destroy(gearSlotObjects[slot]);

            //Debug.Log("try create update gear slot object if key for slot: " + gearSlotKeys.ContainsKey(slot));
            if (gearSlotKeys.ContainsKey(slot))
            {
                int race = WardrobeStuff.raceMap[raceString];
                int sex = WardrobeStuff.genderMap[genderString];
                long key = gearSlotKeys[slot];
                ClothingItem item = new ClothingItem(DBInst.inst, key);
                string nif = item.nifRef.getNif(race, sex);
                //Debug.Log("load nif[" + nif + "] for slot:" + slot);
                gearSlotObjects[slot] = loadNIFForSlot(slot, refModel, costumeParts, Path.GetFileName(nif), "");
            }
        }


        public void setAppearenceSet(long setKey)
        {
            this.appearenceSet = setKey;
            if (this.appearenceSet == long.MinValue)
                gearSlotKeys.Clear();
            if (state == ClassState.IDLE)
                state = ClassState.UPDATE;

        }

        private void loadAppearenceSet()
        {
            if (this.appearenceSet == long.MinValue)
                return;
          
            long setKey = this.appearenceSet;
            if (state != ClassState.UPDATE)
            {
                Debug.LogError("Cannot load appearence [" + setKey + "] without being update mode");
                return;
            }


            // set the ref model to be all visible, overriden parts will be hidden later when parts are added
            SetActiveRecursively(refModel, true);
            // remove all the existing parts
            costumeParts.transform.Clear();

            CObject obj = DBInst.inst.toObj(7638, setKey);
            CObject setParts = obj.getMember(2);

            foreach (CObject part in setParts.members)
            {
                ClothingItem item = new ClothingItem(DBInst.inst, int.Parse(part.convert().ToString()));
                setGearSlotKey(item.allowedSlots.First(), item.key);
            }
            appearenceSet = long.MinValue;
        }

        private GameObject loadNIFForSlot(GearSlot slot, GameObject skeleton, GameObject meshHolder, string nifFile, string geo)
        {
            // First move all the meshes across to the skeleton
            GameObject meshes = new GameObject(slot.ToString());
            try
            {
                NIFFile file = NIFLoader.getNIF(nifFile);
                GameObject newNifRoot = NIFLoader.loadNIF(file, nifFile, true);

                meshes.transform.parent = meshHolder.transform;

                foreach (SkinnedMeshRenderer r in newNifRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    r.transform.parent = meshes.transform;

                // weapons are a bit different
                if (!WardrobeStuff.isWeapon(slot))
                    // process the NiSkinningMeshModifier 
                    NIFLoader.linkBonesToMesh(file, skeleton);
                else
                {
                    /*
                    switch (slot)
                    {
                        case GearSlot.MAIN_HAND:
                        */
                            //newNifRoot.FindDeepChild("propShape").localPosition = Vector3.zero;
                            Transform t = skeleton.transform.FindDeepChild("AP_r_hand");
                            meshes.transform.parent = t;
                            meshes.transform.localPosition = new Vector3(0, 0, 0);
                            //meshes.transform.localPosition = new Vector3(0, 0, 0);
                            //meshes.transform.GetChild(0).localPosition = Vector3.zero;

                    /*
                            break;
                        default:
                            break;

                    }
                    */
                }

                this.animationNif.clearBoneMap();

                // disable the proxy geo
                enableDisableGeo(nifFile, skeleton, false);
                // special case to ensure boots are disabled as well
                if (nifFile.Contains("foot"))
                    enableDisableGeo("boots", skeleton, false);

                //GameObject.DestroyObject(GameObject.Find(geo));
                GameObject.DestroyObject(newNifRoot);
            }
            catch (Exception ex)
            {
                Debug.Log("Exception trying to load nif[" + nifFile + "]" + ex);
            }
            return meshes;
        }

        public void setRace(string race)
        {
            this.raceString = race;
            if (state == ClassState.IDLE)
                state = ClassState.UPDATE;

        }

        public void setGender(string gender)
        {
            this.genderString = gender;
            if (state == ClassState.IDLE)
                state = ClassState.UPDATE;

        }

        public void Update()
        {
           
        }

        public ClassState getState()
        {
            return state;
        }

        public void FixedUpdate()
        {
            if (state == ClassState.UPDATE)
            {
                updateRaceGender();
                loadAppearenceSet();

                checkFemaleModesty();

                foreach (GearSlot g in Enum.GetValues(typeof(GearSlot)))
                    updateGearSlotObject(g);


                state = ClassState.IDLE;
            }


            if(this.animationNif != null)
                animationNif.animSpeed = animSpeed;
        }


        public static void SetActiveRecursively(GameObject rootObject, bool active)
        {
            rootObject.SetActive(active);

            foreach (Transform childTransform in rootObject.transform)
            {
                SetActiveRecursively(childTransform.gameObject, active);
            }
        }

        private static void findChildrenContaining(Transform t, String str, List<Transform> list)
        {
            if (t.name.Contains(str))
                list.Add(t);
            foreach (Transform ct in t)
                findChildrenContaining(ct, str, list);
        }

        private void enableDisableGeo(string nifFile, GameObject skeleton, bool showGeo)
        {
            List<Transform> geoList = new List<Transform>();
            findChildrenContaining(skeleton.transform, "GEO", geoList);

            foreach (string s in nifFile.Split('_'))
            {
                foreach (Transform t in geoList)
                {
                    if (t.name.Contains(s + "_000_GEO") || t.name.Contains(s + "_proxy_GEO"))
                    {
                        t.gameObject.SetActive(showGeo);
                        return;
                    }
                }
            }

        }
    }
}
