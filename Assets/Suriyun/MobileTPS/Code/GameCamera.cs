using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;

namespace Suriyun.MobileTPS
{
    public class GameCamera : MonoBehaviour
    {

        public InputMode input_mode;
        //[HideInInspector]
        public GameObject player;
        public Transform cam_holder;
        public Transform target;
        public Vector3 offset_pos;
        public Vector3 offset_rot;
        public float smoothness = 1.66f;
        private Transform trans;
        public Vector3 target_rotation;

        public Transform aimer;
        public ScreenTransformGesture screen_gesture;
        public float screen_rotation_speed = 0.33f;
        public float screen_rotation_smoothness = 16.66f;

        public bool zoomed = false;
        public float zoomed_speed_multiplier = 0.33f;
        public float zoom_speed = 6f;
        public float fov_zoom = 30;
        public float fov_normal = 60;
        Camera cam;

        void Start()
        {
            trans = transform;
            target_rotation = trans.rotation.eulerAngles;
            aimer.rotation = trans.rotation;
            cam = GetComponent<Camera>();
            player = GameObject.FindObjectOfType<Agent>().gameObject;
#if UNITY_IOS || UNITY_ANDROID
            input_mode = InputMode.MimicTouchscreen;
#endif
        }

        void Update()
        {
            // Zoom control //
            if (zoomed)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov_zoom, zoom_speed * Time.deltaTime);
            }
            else {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov_normal, zoom_speed * Time.deltaTime);
            }

            // Aim position //
            aimer.position = trans.position;

            Vector3 pos = player.transform.position + aimer.forward * offset_pos.z + aimer.up * offset_pos.y + aimer.right * offset_pos.x;
            pos.y = Mathf.Clamp(pos.y, player.transform.position.y, pos.y + 1);
            trans.position = Vector3.Slerp(
                trans.position,
                pos,
                smoothness * Time.deltaTime);

            RaycastHit hit;
            if (Physics.Raycast(trans.position, trans.forward, out hit))
            {
                target.transform.position = hit.point;
            }
            else {
                target.transform.position = trans.position + trans.forward * 20;
            }

            // Rotation control //
            aimer.localRotation = Quaternion.Euler(aimer.localRotation.eulerAngles.x, aimer.localRotation.eulerAngles.y, 0);
            trans.rotation = Quaternion.Slerp(trans.rotation, aimer.rotation, screen_rotation_smoothness * Time.deltaTime);
        }

        public bool mouse_aiming = false;
        public void StartMouseAim()
        {
#if UNITY_WEBGL || UNITY_STANDALONE
            mouse_aiming = true;
            StartCoroutine(UpdateMouse());
            Cursor.lockState = CursorLockMode.Locked;
#endif
        }
        public void StopMouseAim()
        {
#if UNITY_WEBGL || UNITY_STANDALONE
            mouse_aiming = false;
#endif
        }

        public void ShowCursor()
        {
#if UNITY_WEBGL || UNITY_STANDALONE
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
#endif
        }

        public void HideCursor()
        {
#if UNITY_WEBGL || UNITY_STANDALONE
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
#endif
        }

        private void OnEnable()
        {
            screen_gesture.Transformed += FullScreenHandler;
        }

        private void OnDisable()
        {
            screen_gesture.Transformed -= FullScreenHandler;
        }

        private void FullScreenHandler(object sender, System.EventArgs e)
        {
            if (this.input_mode == InputMode.MimicTouchscreen)
            {
                int most_right_pointers = 0;
                TouchScript.Pointers.Pointer most_right_pointer = screen_gesture.ActivePointers[0];
                for (int i = 1; i < screen_gesture.ActivePointers.Count; i++)
                {
                    if (screen_gesture.ActivePointers[i].Position.x > screen_gesture.ActivePointers[i - 1].Position.x)
                    {
                        most_right_pointer = screen_gesture.ActivePointers[i];
                    }
                }
                

                this.HandleInput(screen_gesture.DeltaPosition.x, (-1f) * screen_gesture.DeltaPosition.y);
            }
        }

        public float mouse_sensitivity = 0.6f;
        public void ModifyMouseSensitivity(float amount)
        {
            mouse_sensitivity += amount;
        }

#if UNITY_WEBGL || UNITY_STANDALONE

        IEnumerator UpdateMouse()
        {
            Vector3 lastframe_pos = Input.mousePosition;
            Vector3 delta_pos = Vector3.zero;

            while (mouse_aiming)
            {
                if (this.input_mode == InputMode.Mouse)
                {//-----------------------------------------------------------------------

                    delta_pos = lastframe_pos - Input.mousePosition;
                    lastframe_pos = Input.mousePosition;

                    delta_pos.x *= -1f;

                    this.HandleInput(
                        Input.GetAxis("Mouse X") * mouse_sensitivity,
                        Input.GetAxis("Mouse Y") * mouse_sensitivity * (-1f)
                    );
                    this.HandleMouseClick();
                }//-----------------------------------------------------------------------
                yield return 0;
            }
        }

        [HideInInspector]
        public Agent agent;

        public void HandleMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (agent == null)
                {
                    agent = player.GetComponent<Agent>();
                }
                agent.behaviour.StartFiring();
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (agent == null)
                {
                    agent = player.GetComponent<Agent>();
                }
                agent.behaviour.StopFiring();
            }
        }
#endif

        public void HandleInput(float delta_x, float delta_y)
        {
            // Aim camera //
            Vector3 rotate_horizontal = Vector3.up * delta_x * screen_rotation_speed;
            Vector3 rotate_vertical = aimer.right * delta_y * screen_rotation_speed;

            if (zoomed)
            {
                rotate_horizontal *= zoomed_speed_multiplier;
                rotate_vertical *= zoomed_speed_multiplier;
            }

            Quaternion tmp = aimer.rotation;
            aimer.Rotate(rotate_vertical, Space.World);
            if (aimer.up.y < 0)
            {
                aimer.rotation = tmp;
            }
            aimer.Rotate(rotate_horizontal, Space.World);

        }

        public void SwitchInputMode()
        {
            if (input_mode == InputMode.MimicTouchscreen)
            {
                input_mode = InputMode.Mouse;
            }
            else if (input_mode == InputMode.Mouse)
            {
                input_mode = InputMode.MimicTouchscreen; ;
            }
        }
    }

    public enum InputMode
    {
        MimicTouchscreen,
        // For mobile device testing on desktop
        Mouse
        // For desktop game
    }

}