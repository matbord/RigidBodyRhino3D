using System;
using System.Collections.Generic;
using Jitter.Dynamics;
using Rhino.Geometry;
using Rhino;
using Jitter;
using Jitter.Collision;
using Rhino.DocObjects;
using Jitter.LinearMath;

namespace RigidBodyRhino
{
    class RigidBodyEvents
    {
        public RigidBodyEvents()
        {
            RhinoDoc.SelectObjects += OnSelectObjects;
            RhinoDoc.DeselectAllObjects += DeselectObjects;
            RhinoDoc.DeleteRhinoObject += DeleteObjects;
            //Save transformation in list
            RhinoDoc.BeforeTransformObjects += RhinoDocOnBeforeTransformObjects;
            
            RhinoDoc.NewDocument += RhinoNewDoc;
            RhinoDoc.EndOpenDocumentInitialiViewUpdate += EndOpen;
            RhinoDoc.ReplaceRhinoObject += ReplaceRhinoObject;
        }

        void ReplaceRhinoObject(object sender, RhinoReplaceObjectEventArgs args) {
            //if it is not a rigid transformation avoid this transformation
            if (!rigidTransf)
            {
                rigidTransf = true;
                RhinoDoc.ActiveDoc.Objects.Replace(args.ObjectId, (Brep)args.OldRhinoObject.Geometry);
            }          
        }

        void OnSelectObjects(object sender, RhinoObjectSelectionEventArgs args)
        {
            if (args.Selected) // objects were selected
            {
                //Control if all selected RhinoObjects are static (if only one object is not static or not a jittershape, uncheck the box)
                bool isStatic = true;
                bool jitterShape = true;
                for (int i = 0; i < args.RhinoObjects.Length; i++)
                {
                    RhinoObject obj = args.RhinoObjects[i];
                    Guid id = obj.Id;
                    if (RigidBodyManager.GuidList.Contains(id))
                    {
                        int index = RigidBodyManager.GuidList.IndexOf(id);
                        if (!RigidBodyManager.RigidBodies[index].IsStatic)
                            isStatic = false;
                    }
                    else
                    {
                        jitterShape = false;
                    }
                }
                //If all RhinoObjects are static set check otherwise uncheck
                TimePanel.Instance.IsStaticCheckBoxChecked = isStatic;

                //if there is minimum one selected shape that is not a jitter shape, so disable the checkbox
                TimePanel.Instance.IsStaticCheckBoxEnabled = jitterShape;

                //Now show the checked box with the correct value
                TimePanel.Instance.PropertiesGroupShow();
            }
        }

        void DeselectObjects(object sender, RhinoDeselectAllObjectsEventArgs args)
        {
            TimePanel.Instance.PropertiesGroupHide();
        }

        void DeleteObjects(object sender, RhinoObjectEventArgs args)
        {
            TimePanel.Instance.PropertiesGroupHide();
        }

        private void EndOpen(object sender, DocumentOpenEventArgs e)
        {
            //Add shapes to the world after open the saved file
            for (int i = 0; i < RigidBodyRhinoPlugIn.WorldCount; i++)
            {
                RigidBody currentRigidBody = RigidBodyManager.RigidBodies[i];
                RigidBodyManager.World.AddBody(currentRigidBody);
                TimePanel.Instance.DrawGeometry(i, currentRigidBody);
            }
            for (int i = 0; i < RigidBodyManager.GuidList.Count; i++)
            {
                RhinoDoc.ActiveDoc.Objects.Unlock(RigidBodyManager.GuidList[i], true);
            }
        }


        bool rigidTransf = true;
        private void RhinoDocOnBeforeTransformObjects(object sender, RhinoTransformObjectsEventArgs ea)
        {
            RhinoObject[] rhinoObjects = ea.Objects;
            Vector3d traslation;
            Transform transform;

            ea.Transform.DecomposeAffine(out transform, out traslation);
            transform = transform.Transpose();
            
            for (int i = 0; i < rhinoObjects.Length; i++)
            {
                //Se il guid é presente nella lista allora fa parte delle jitter forme
                if (RigidBodyManager.GuidList.Contains(rhinoObjects[i].Id))
                {
                    int index = RigidBodyManager.GuidList.IndexOf(rhinoObjects[i].Id);
                    //If it is a rigid transformation (so translation and rotation)
                    if (ea.Transform.RigidType == TransformRigidType.Rigid)
                    {   //Rotate the body
                        JMatrix rotation = RigidBodyManager.RigidBodies[index].Orientation * new JMatrix((float)transform.M00, (float)transform.M01, (float)transform.M02, (float)transform.M10, (float)transform.M11, (float)transform.M12, (float)transform.M20, (float)transform.M21, (float)transform.M22);
                        RigidBodyManager.RigidBodies[index].Orientation = rotation;
                        //Move the center of mass of Jitter shape on the center of the BBox of rhino shape
                        Brep rhinoobj = (Brep)(ea.Objects[i].Geometry).Duplicate();
                        rhinoobj.Transform(ea.Transform);
                        Point3d centerBbox = rhinoobj.GetBoundingBox(true).Center;
                        RigidBodyManager.RigidBodies[index].Position = RigidBodyManager.Point3dtoJVector(centerBbox);
                        //Find the difference between rhino bbx center and jitter bbox center
                        JVector bboxjitter = RigidBodyManager.RigidBodies[index].BoundingBox.Center;
                        JVector diff = bboxjitter - RigidBodyManager.Point3dtoJVector(centerBbox);
                        //Align the center of both bboxes
                        RigidBodyManager.RigidBodies[index].Position -= diff;
                    }
                    else {
                        RhinoApp.WriteLine("You can't apply a non rigid transformation to a shape that has a rigid body property");
                        rigidTransf = false;
                    }
                }
            }
        }

        private void RhinoNewDoc(object sender, DocumentEventArgs ea)
        {
            RigidBodyManager.RigidBodies = new List<RigidBody>();
            RigidBodyManager.GuidList = new List<Guid>();
            RigidBodyManager.GeometryList = new List<Brep>();
            RigidBodyManager.World = new World(new CollisionSystemSAP());
            TimePanel.Instance.MaxFrameBoxValue = 100;
            TimePanel.Instance.ResetNotSavable();
        }
    }
}
