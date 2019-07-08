using Rhino.UI;
using Rhino.PlugIns;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Jitter.LinearMath;
using Rhino.FileIO;
using System;
using Jitter.Dynamics;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Jitter;
using Jitter.Collision;
using Rhino.DocObjects.Tables;

namespace RigidBodyRhino
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class RigidBodyRhinoPlugIn : Rhino.PlugIns.PlugIn

    {
        public RigidBodyRhinoPlugIn()
        {
            Instance = this;
            Panels.RegisterPanel(Instance, typeof(TimePanel), LOC.STR("TimePanel"), null, PanelType.System);
            RigidBodyEvents events = new RigidBodyEvents();
        }

        ///<summary>Gets the only instance of the RigidBodyRhinoPlugIn plug-in.</summary>
        public static RigidBodyRhinoPlugIn Instance
        {
            get; private set;
        }

        protected override bool ShouldCallWriteDocument(FileWriteOptions options)
        {
            if (TimePanel.Instance.Restarted && TimePanel.Instance.TrackbarValue == 0)
                return true;
            else
            {
                Dialogs.ShowMessage("You can't save JShapes when you are running an animation so it will be save the starting positions.", "Warning during saving", ShowMessageButton.OK, ShowMessageIcon.Warning);
                return true;
            }
        }

        protected override void WriteDocument(RhinoDoc doc, BinaryArchiveWriter archive, FileWriteOptions options)
        {

            var dict = new Rhino.Collections.ArchivableDictionary(1, "RigiBodyData");
            //If user save during animation, save the shapes at the time when start was pressed
            if (!TimePanel.Instance.Restarted || TimePanel.Instance.TrackbarValue != 0)
            {
                List<RigidBody> copyToAdd = new List<RigidBody>(); ;
                for (int i = 0; i < RigidBodyManager.World.RigidBodies.Count; i++)
                {
                    copyToAdd.Add(RigidBodyManager.DuplicateRigidBody(RigidBodyManager.FramesRigidBodies[0][i], RigidBodyManager.FramesRigidBodies[0][i].IsStatic));
                }
                dict.Set("rigidBodies", RigidBodyManager.ObjectToByteArray(copyToAdd));
            }
            else
                dict.Set("rigidBodies", RigidBodyManager.ObjectToByteArray(RigidBodyManager.RigidBodies));

            dict.Set("guidList", RigidBodyManager.ObjectToByteArray(RigidBodyManager.GuidList));
            dict.Set("geometryList", RigidBodyManager.ObjectToByteArray(RigidBodyManager.GeometryList));
            dict.Set("MaxFrameBoxValue", (int)TimePanel.Instance.MaxFrameBoxValue);
            dict.Set("worldCount", RigidBodyManager.World.RigidBodies.Count);
            archive.WriteDictionary(dict);
        }
        public static int WorldCount { get; private set; }

        protected override void ReadDocument(RhinoDoc doc, BinaryArchiveReader archive, FileReadOptions options)
        {
            RigidBodyManager.World = new World(new CollisionSystemSAP());
            Rhino.Collections.ArchivableDictionary dict = archive.ReadDictionary();
            RigidBodyManager.GuidList = (List<Guid>)RigidBodyManager.ByteArrayToObject(dict["guidList"] as byte[]);
            RigidBodyManager.RigidBodies = (List<RigidBody>)RigidBodyManager.ByteArrayToObject(dict["rigidBodies"] as byte[]);
            RigidBodyManager.GeometryList = (List<Brep>)RigidBodyManager.ByteArrayToObject(dict["geometryList"] as byte[]);
            TimePanel.Instance.MaxFrameBoxValue = (int)dict["MaxFrameBoxValue"];
            //Reset
            TimePanel.Instance.ResetNotSavable();
            WorldCount = (int)dict["worldCount"];
        }
    }
}