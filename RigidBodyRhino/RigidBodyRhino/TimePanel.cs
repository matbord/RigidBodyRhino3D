using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Jitter.Dynamics;
using Rhino.Geometry;
using Rhino;
using Rhino.DocObjects;

namespace RigidBodyRhino
{
    [Guid("5B59C560-D836-4A7A-B207-B690D18AFDDE")]
    public partial class TimePanel : UserControl
    {
        private static bool pause = true;
        private static bool restarted = true;
        public bool Restarted
        {
            get
            {
                return restarted;
            }
        }
        private static int maxFrame = 100;

        //Created to save the max frame that user want
        public decimal MaxFrameBoxValue
        {
            set
            {
                MaxFrameBox.Value = value;
            }
            get
            {
                return MaxFrameBox.Value;
            }
        }

        public int TrackbarValue
        {
            get
            {
                return FrameTrackBar.Value;
            }
        }

        public bool IsStaticCheckBoxChecked
        {
            set
            {
                IsStaticCheckBox.Checked= value;
            }
        }

        public bool IsStaticCheckBoxEnabled
        {
            set
            {
                IsStaticCheckBox.Enabled = value;
            }
        }

        public void PropertiesGroupShow()
        {
            PropertiesGroup.Show();
        }

        public void PropertiesGroupHide()
        {
            PropertiesGroup.Hide();
        }

        //Singleton
        private static readonly object padlock = new object();
        private TimePanel instance = null;
        public TimePanel Instance
        {
            get
            {
               return instance;
            }
        }
        /*public static TimePanel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new TimePanel();
                        instance.InitializeComponent();
                    }
                    return instance;
                }
            }
        }*/

        public TimePanel()
        {
            instance = this;
            //Inizialize components
            InitializeComponent();
        }
        //This is execute just the panel is open
        private void TimePanel_Load(object sender, EventArgs e)
        {
            Restart.Enabled = !pause;
            FrameTrackBar.SetRange(0, 99);
            FrameTrackBar.BackColor = System.Drawing.Color.WhiteSmoke;
            FrameProgressBar.Minimum = 0;
            FrameProgressBar.Maximum = 99;
            MaxFrameBox.Minimum = 100;
            MaxFrameBox.Maximum = 500;
            PropertiesGroup.Hide();
            Play.Enabled = pause;
            Pause.Enabled = !pause;
            FrameTrackBar.Enabled = false;
        }     

        private void Play_Click(object sender, EventArgs e)
        {
            //Get max frames from panel label
            maxFrame = (int)MaxFrameBox.Value;
            FrameTrackBar.Maximum =(int) MaxFrameBox.Value-1;
            FrameProgressBar.Maximum = FrameTrackBar.Maximum;
            MaxFrameBox.Enabled = false;
            //I focuse on pause beacuse when I run Play the button Play remain focused. 
            //With this command I remove the focus on the Play button
            Pause.Focus();
            pause = false;
            Restart.Enabled = pause;
            Play.Enabled = pause;
            Pause.Enabled = !pause;
            FrameTrackBar.Enabled = false;

            if (restarted)
            {
                //If user delete some obects, remove them from lists
                for (int i = 0; i < RigidBodyManager.World.RigidBodies.Count; i++)
                {
                    Guid currentGuid = RigidBodyManager.GuidList[i];
                    RhinoObject currentRhinoObject = RhinoDoc.ActiveDoc.Objects.Find(currentGuid);
                    if (currentRhinoObject == null)
                    {
                        DeleteJitterObject(i);
                        i--;
                    }
                }
            }

            if (RigidBodyManager.World.RigidBodies.Count != 0)
            {
                for (int f = FrameTrackBar.Value; f < maxFrame && !pause; f++)
                {
                    FrameTrackBar.Value = f;
                    ActualFrame.Text = "Frame " + f;
                    // If it is the first time, it saves the frames otherwise read them
                    if (restarted)
                    {  
                        //Array of rigid bodies in this single frame
                        List<RigidBody> rigidBodyFrame = new List<RigidBody>(RigidBodyManager.World.RigidBodies.Count);
                        for (int i = 0; i < RigidBodyManager.World.RigidBodies.Count; i++)//for every rigid body in the world
                        {
                            RigidBody body = RigidBodyManager.RigidBodies[i];
                            DrawGeometry(i,body);

                            //Add the rigidBody form into the array that contains every rigidBody of a frame
                            rigidBodyFrame.Add(RigidBodyManager.DuplicateRigidBody(body, body.IsStatic));

                        }
                        //you can interact with the doc during the simulation 
                        RhinoApp.Wait();
                        RhinoDoc.ActiveDoc.Views.Redraw();
                        RigidBodyManager.World.Step(1.0f / 25.0f, true);

                        //Add the RigidBody array of this single frame to the list that contains all frames
                        RigidBodyManager.FramesRigidBodies.Add(rigidBodyFrame);
                    }
                    else
                    {//Only read frames and not save them
                        for (int i = 0; i < RigidBodyManager.World.RigidBodies.Count; i++)//The number of rigid bodies in the world is equal to the lenght of the array in every frame (frames are rappresented by framesRigidBodies)
                        {
                            RigidBody bodyOfFrame = RigidBodyManager.FramesRigidBodies[f][i];
                            DrawGeometry(i,bodyOfFrame);
                        }
                        RhinoApp.Wait();
                        RhinoDoc.ActiveDoc.Views.Redraw();
                    }
                }
      
                if (Pause.Enabled)
                    Pause_Click(null, null);

                if (FrameTrackBar.Value == maxFrame-1) { 
                    Play.Enabled=false;
                    restarted = false;
                    FrameTrackBar.Enabled = true;
                }
            }            
        }

        private void Pause_Click(object sender, EventArgs e)
        {
            for(int i=0;i< RigidBodyManager.World.RigidBodies.Count;i++)
                RhinoDoc.ActiveDoc.Objects.Lock(RigidBodyManager.GuidList[i], true);
            pause = true;
            Restart.Enabled = pause;
            Play.Enabled = pause;
            Pause.Enabled = !pause;
            FrameTrackBar.Enabled = true;
            if (restarted)
                FrameTrackBar.Enabled = false;
        }

        private void Restart_Click(object sender, EventArgs e)
        {
            restarted = true;
            Restart.Enabled = !pause;
            Pause.Enabled = !pause;
            Play.Enabled = pause;           
            FrameTrackBar.Value = 0;
            FrameTrackBar.Enabled = false;
            MaxFrameBox.Enabled = true;
            ActualFrame.Text = "Frame 0";
            for (int i = 0; i < RigidBodyManager.World.RigidBodies.Count; i++)
            {
                //I remove the actual and I insert another
                RigidBodyManager.World.RemoveBody(RigidBodyManager.RigidBodies[i]);
                RigidBody rigidCopyToAdd = RigidBodyManager.DuplicateRigidBody(RigidBodyManager.FramesRigidBodies[0][i], RigidBodyManager.FramesRigidBodies[0][i].IsStatic);
                RigidBodyManager.World.AddBody(rigidCopyToAdd);
                RigidBodyManager.RigidBodies[i] = rigidCopyToAdd;
                DrawGeometry(i, rigidCopyToAdd);
                RhinoDoc.ActiveDoc.Objects.Unlock(RigidBodyManager.GuidList[i], true);
            }
            RigidBodyManager.FramesRigidBodies.Clear();
        }

        private void IsStaticCheckBox_Click(object sender, EventArgs e)
        {
            foreach (RhinoObject obj in RhinoDoc.ActiveDoc.Objects.GetSelectedObjects(false, false))
            {
                Guid id = obj.Id;
                if (RigidBodyManager.GuidList.Contains(id))
                {
                    int index = RigidBodyManager.GuidList.IndexOf(id);
                    RigidBodyManager.RigidBodies[index].IsStatic = IsStaticCheckBox.Checked;
                }
            }
        }

        private void TimePanel_SizeChanged(object sender, EventArgs e)
        {
            //Resize the trackbar when the panel resize
            FrameTrackBar.Width = Instance.Width - 20;
            FrameProgressBar.Width= Instance.Width - 44;
            TimeBarBox.Width = Instance.Width - 14;
        }

        private void FrameTrackBar_ValueChanged(object sender, EventArgs e)
        {
            ProgressBarAnimation(FrameTrackBar.Value);

            if (Restart.Enabled)
            {
                if(FrameTrackBar.Value!=maxFrame-1)
                    Play.Enabled=true;
                else
                    Play.Enabled = false;

                //Blocks who want future frames that wasn't created(so only the first time they try the simulation)
                if (FrameTrackBar.Value > RigidBodyManager.FramesRigidBodies.Count-1)
                    FrameTrackBar.Value = RigidBodyManager.FramesRigidBodies.Count-1;               
                
                int index = FrameTrackBar.Value;
                ActualFrame.Text="Frame "+ index;
                for (int i = 0; i < RigidBodyManager.World.RigidBodies.Count; i++)//The number of rigid bodies in the world is equal to the lenght of the array in every frame (frames are rappresented by framesRigidBodies)
                {
                    RigidBody bodyOfFrame = RigidBodyManager.FramesRigidBodies[index][i];
                    DrawGeometry(i,bodyOfFrame);
                }
            }
            
        }

        private void RigidBodyButton_Click(object sender, EventArgs e)
        {
            RhinoApp.ExecuteCommand(RhinoDoc.ActiveDoc, "RigidBody");
        }

        private void JBoxButton_Click(object sender, EventArgs e)
        {
            RhinoApp.ExecuteCommand(RhinoDoc.ActiveDoc, "JBox");
        }

        private void JShpereButton_Click(object sender, EventArgs e)
        {
            RhinoApp.ExecuteCommand(RhinoDoc.ActiveDoc, "JSphere");
        }

        private void JCylinderButton_Click(object sender, EventArgs e)
        {
            RhinoApp.ExecuteCommand(RhinoDoc.ActiveDoc, "JCylinder");
            
        }

        private void JCompound_Click(object sender, EventArgs e)
        {
            RhinoApp.ExecuteCommand(RhinoDoc.ActiveDoc, "JCompoundShape");
        }

        public void DrawGeometry(int i, RigidBody body)
        {
            RhinoDoc.ActiveDoc.Objects.Unlock(RigidBodyManager.GuidList[i], true);
            Brep currentBrep = RigidBodyManager.GeometryList[i];

            //Create copy of Brep
            Brep copyToAdd = currentBrep.DuplicateBrep();
            //Rotate the rhino object
            Transform trafo = RigidBodyManager.MatrixTransfRotation(body);
            trafo = trafo.Transpose();
            copyToAdd.Transform(trafo);

            //Translate and so now the center of the bounding boxes are the same
            copyToAdd.Translate(body.BoundingBox.Center.X, body.BoundingBox.Center.Y, body.BoundingBox.Center.Z);

            RhinoDoc.ActiveDoc.Objects.Replace(RigidBodyManager.GuidList[i], copyToAdd);
            RhinoDoc.ActiveDoc.Objects.Lock(RigidBodyManager.GuidList[i], true);
        }

        private void DeleteJitterObject(int i)
        {
            RigidBodyManager.GeometryList.RemoveAt(i);
            RigidBodyManager.GuidList.RemoveAt(i);
            RigidBodyManager.World.RemoveBody(RigidBodyManager.RigidBodies[i]);
            RigidBodyManager.RigidBodies.RemoveAt(i);
            for (int k = 0; k < RigidBodyManager.FramesRigidBodies.Count; k++)
            {
                RigidBodyManager.FramesRigidBodies[k].RemoveAt(i);
            }
        }

        //Reset attributes for load or new document
        public void ResetNotSavable()
        {
            FrameTrackBar.Value = 0;
            RigidBodyManager.FramesRigidBodies.Clear();
            pause = true;
            restarted = true;
            ActualFrame.Text = "Frame 0";
            maxFrame = 100;
            Play.Enabled = true;
            Pause.Enabled = false;
            Restart.Enabled = false;
        }

        //Animation of load bar
        private void ProgressBarAnimation(int f)
        {
            if (f == FrameProgressBar.Maximum)
            {
                FrameProgressBar.Maximum = FrameProgressBar.Maximum + 1;
                FrameProgressBar.Value = f + 1;
                FrameProgressBar.Value = FrameProgressBar.Value - 1;
                FrameProgressBar.Maximum = FrameProgressBar.Maximum - 1;
            }
            else
            {
                FrameProgressBar.Value = f + 1;
                FrameProgressBar.Value = FrameProgressBar.Value - 1;
            }
        }
    }
}
