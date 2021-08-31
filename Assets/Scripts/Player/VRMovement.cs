using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRMovement : MonoBehaviour
{
    public XRNode leftInputSource;
    public XRNode rightInputSource;
    public XRRig rig;
    public CharacterController character;
    public float additionalCharacterHeight = 0.2f;
    public int speed;
    private float fallingVelocity;
    private Vector2 inputAxis;
    private float gravity = -9.81f;

    void Update() {
        // Left hand for movement
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(leftInputSource);
        leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    void FixedUpdate()
    {
        CapsuleFollowHeadset();

        Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        character.Move(direction * Time.fixedDeltaTime * speed);

        // Deal with gravity.
        bool grounded = isGrounded();
        if (grounded) {
            fallingVelocity = 0;
        } else {
            fallingVelocity += gravity * Time.fixedDeltaTime;
        }

        character.Move(Vector3.up * fallingVelocity * Time.fixedDeltaTime);
    }

    void CapsuleFollowHeadset() {
        character.height = rig.cameraInRigSpaceHeight + additionalCharacterHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height/2 + character.skinWidth, capsuleCenter.z);
    }

    bool isGrounded() {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool rayHasHit = Physics.SphereCast(rayStart, character.radius - 0.2f, Vector3.down, out RaycastHit hitInfo, rayLength);
        return rayHasHit;
    }
}
