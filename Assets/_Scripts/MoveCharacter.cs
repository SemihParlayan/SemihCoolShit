using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour 
{
    public enum MoveState { Moving, Swinging, Glide }

    //public variables
    public bool debug;
    public MoveState moveState = MoveState.Moving;
    public float gravity;
    public float externalVelocityDamping;
    public float slopeRayLength;
    public float slopeForce;
    public Transform shadowTransform;
    public Transform modelTransform;
    [FoldoutGroup("Ground move")] public float movementSpeed;
    [FoldoutGroup("Ground move")] public float sprintSpeed;
    [FoldoutGroup("Ground move")] public float jumpSpeed;
    [FoldoutGroup("Ground move")] public float rotateSpeed;
    [FoldoutGroup("Ground move")] public float tiltSpeed;
    [FoldoutGroup("Ground move")] public float tiltModifier;
    [FoldoutGroup("Ground move")] public Vector3 dashForce;
    [FoldoutGroup("Swinging")] public Transform lineStartPoint;
    [FoldoutGroup("Swinging")] public VerletRope verletRopePrefab;
    [FoldoutGroup("Swinging")] public float findSwingDistance;
    [FoldoutGroup("Swinging")] public float maxSwingMagnitude;
    [FoldoutGroup("Swinging")] public float swingGravity;
    [FoldoutGroup("Swinging")] public float swingControlSpeed;
    [FoldoutGroup("Swinging")] public float swingSpeed;
    [FoldoutGroup("Swinging")] public float swingRotateSpeed;
    [FoldoutGroup("Swinging")] public float swingRopeRemoveLength;
    [FoldoutGroup("Swinging")] public float shootRopeTime;
    [FoldoutGroup("Swinging")] public float stopSwingForce;
    [FoldoutGroup("Swinging")] public float ropeBounceSpeed;
    [FoldoutGroup("Swinging")] public float ropeBounceWidth;
    [FoldoutGroup("Swinging")] public float ropeBounceFrequency;
    [FoldoutGroup("Swinging")] public AnimationCurve ropeLerpSpeed;
    [FoldoutGroup("Glide")] public Animator gliderAnimator;
    [FoldoutGroup("Glide")] public GameObject wingPrefab;
    [FoldoutGroup("Glide")] public Transform wingPos1;
    [FoldoutGroup("Glide")] public Transform wingPos2;
    [FoldoutGroup("Glide")] public float glideGravity;
    [FoldoutGroup("Glide")] public float minGlideYVelocity;
    [FoldoutGroup("Glide")] public float maxGlideSpeed;
    [FoldoutGroup("Glide")] public float minGlideAcceleration;
    [FoldoutGroup("Glide")] public float maxGlideAcceleration;
    [FoldoutGroup("Glide")] public float glideRotateSpeed;
    [FoldoutGroup("Glide")] public float glideTurnSpeed;
    [FoldoutGroup("Squisher")] public Squisher squisher;
    [FoldoutGroup("Squisher")] public SO_SquisherSettings squish_Land;

    public Transform modelPivot;
    public CharacterController controller;
    public LineRenderer swingLine;
    public LayerMask groundMask;
    public ParticleSystem runParticles;
    public ParticleSystem landParticles;

    //private variables
    private InputManager inputManager;
    private Vector3 movement = Vector3.zero;
    private Vector3 prevMovement;
    private Vector3 transformCrossRight = Vector3.zero;
    private Vector3 transformCrossForward = Vector3.zero;
    private RaycastHit groundHit;
    private float yVelocity = 0f;
    private bool groundedLastFrame = true;
    private SwingPoint swingPoint;
    private float startRopeLength;
    private float ropeLength;
    private int AvailableJumps { get { return availableJumps; } set { availableJumps = Mathf.Clamp(value, 0, int.MaxValue); } }
    private int availableJumps = 1;
    private Vector3 prevSwingMovement;
    private Vector3 externalVelocity;
    private Coroutine attackRopeRoutine;
    private Vector3 glideDirection;
    private float currentGlideSpeed;
    private bool recentlyBounced;
    private bool groundedRay;

    //unity methods
    private void Awake()
    {
        ComponentManager<MoveCharacter>.Value = this;
        inputManager = GetComponent<InputManager>();
    }
    private void Update()
    {
        Application.targetFrameRate = 60;

        inputManager.UpdateInput();

        //Ground raycast
        groundedRay = false;
        if (Physics.Raycast(transform.position + transform.up * 0.5f, -Vector3.up, out groundHit, 200f, groundMask))
        {
            groundedRay = groundHit.distance <= slopeRayLength;
            if (debug)
            {
                Debug.DrawRay(transform.position + transform.up * 0.5f, -Vector3.up, Color.white, 0.5f);
                Debug.DrawRay(groundHit.point, groundHit.normal * 0.25f, Color.magenta, 0.5f);
            }
        }

        //Shadow
        shadowTransform.position = groundHit.point + groundHit.normal * 0.05f;
        shadowTransform.forward = -groundHit.normal;

        //Calculate new right and forward vector based on ground normal and input
        transformCrossRight = Vector3.Cross(inputManager.lastValidMoveAxis, groundHit.normal);
        transformCrossForward = -Vector3.Cross(transformCrossRight, groundHit.normal);

        //Start with no movement direction
        movement = Vector3.zero;

        //Handle different movements
        switch (moveState)
        {
            case MoveState.Moving:
                HandleGroundState();
                break;
            case MoveState.Swinging:
                HandleSwingState();
                break;
            case MoveState.Glide:
                HandleGlideState();
                break;
        }

        //Apply movement to controller
        Vector3 finalMovement = Vector3.zero;
        if (movement != Vector3.zero || externalVelocity != Vector3.zero)
        {
            finalMovement = movement;
            finalMovement += externalVelocity;
            controller.Move(finalMovement * Time.deltaTime);

            if (OnSlope())
            {
                controller.Move(Vector3.down * controller.height / 2f * slopeForce * Time.deltaTime);
            }
        }
        prevMovement = finalMovement;

        //Reduce external velocity;
        float damping = controller.isGrounded ? externalVelocityDamping * 2f : externalVelocityDamping;
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, Time.deltaTime * damping);

        //Run particles
        runParticles.enableEmission = controller.isGrounded && moveState == MoveState.Moving;

        groundedLastFrame = controller.isGrounded;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("Danger"))
        {
            GetComponent<Entity>().Kill();
        }
        else if (hit.transform.CompareTag("Bouncepad"))
        {
            HandleBouncepad(hit);
        }
        else if (hit.transform.CompareTag("Pickup"))
        {
            HandlePickup(hit);
        }
    }

    //public methods
    public bool ChangeMoveState(MoveState newState)
    {
        if (newState == moveState)
            return false;

        //Exit current state
        switch (moveState)
        {
            case MoveState.Moving:
                break;
            case MoveState.Swinging:
                if (attackRopeRoutine != null)
                    StopCoroutine(attackRopeRoutine);
                swingLine.enabled = false;
                if (!controller.isGrounded)
                {
                    AvailableJumps = 1;
                }

                VerletRope oldRope = GameObject.Instantiate(verletRopePrefab, swingPoint.transform.position, Quaternion.identity) as VerletRope;
                oldRope.Initialize(swingPoint.transform.position, transform.position);

                break;
            case MoveState.Glide:
                gliderAnimator.SetBool("Open", false);
                externalVelocity = glideDirection * currentGlideSpeed;
                if (!recentlyBounced)
                    yVelocity = 0f;

                GameObject wing1 = GameObject.Instantiate(wingPrefab, wingPos1.position, wingPos1.rotation) as GameObject;
                wing1.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                wing1.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)).normalized * 10f);
                Destroy(wing1, 60f);
                GameObject wing2 = GameObject.Instantiate(wingPrefab, wingPos2.position, wingPos2.rotation) as GameObject;
                wing2.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                wing2.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)).normalized * 10f);
                Destroy(wing2, 60f);
                break;
        }

        //Enter new state
        switch (newState)
        {
            case MoveState.Moving:
                break;
            case MoveState.Swinging:
                ResetTilt();
                yVelocity = 0f;
                swingLine.enabled = true;
                startRopeLength = Vector3.Distance(transform.position, swingPoint.transform.position);
                ropeLength = startRopeLength;
                prevSwingMovement = prevMovement;
                if (prevSwingMovement.y > 0)
                    prevSwingMovement.y = 0;
                swingLine.SetPosition(0, lineStartPoint.position);
                swingLine.SetPosition(1, lineStartPoint.position);
                attackRopeRoutine = StartCoroutine(AttachRope(swingPoint.transform.position));
                break;
            case MoveState.Glide:
                gliderAnimator.SetBool("Open", true);
                ResetTilt();
                glideDirection = (transform.forward + externalVelocity).normalized;
                glideDirection.y = 0;
                currentGlideSpeed = inputManager.button_sprint ? sprintSpeed : movementSpeed;

                if (yVelocity < 0)
                    yVelocity /= 4f;
                break;
        }
        moveState = newState;
        return true;
    }

    //private methods
    private bool OnSlope()
    {
        if (yVelocity > 0)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + controller.center, -transform.up, out hit, controller.height / 2f * slopeRayLength))
        {
            if (hit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }
    private void HandleGravity(float gravity)
    {
        if (!controller.isGrounded)
        {
            yVelocity += gravity * Time.deltaTime;
        }
        else
        {
            Land(Mathf.Abs(yVelocity));

            if (yVelocity < -1)
            {
                yVelocity = -1;
            }
        }
    }
    private void HandleModelRotation()
    {
        //Rotate transform towards surface up and input forward
        //Also stores what angle we turned in this frame
        Quaternion targetRotation = Quaternion.LookRotation(transformCrossForward, groundHit.normal);
        Vector3 prevForward = transform.forward;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        float turnAngle = Vector3.SignedAngle(prevForward, transform.forward, transform.up);


        //Tilt character when turning
        Vector3 localEuler = modelPivot.localEulerAngles;
        localEuler.z = Mathf.LerpAngle(localEuler.z, turnAngle * tiltModifier, Time.deltaTime * tiltSpeed);
        modelPivot.localEulerAngles = localEuler;
    }
    private void ResetTilt()
    {
        modelPivot.localEulerAngles = Vector3.zero;
    }
    private IEnumerator AttachRope(Vector3 target)
    {
        float startTime = Time.time;
        float normalizedTime = 0f;

        int linePosCount = swingLine.positionCount;

        while (normalizedTime < 1f)
        {
            normalizedTime = Mathf.Clamp01((Time.time - startTime) / shootRopeTime);

            Vector3 lineStart = lineStartPoint.position;
            Vector3 targetPos = Vector3.Lerp(lineStart, target, Mathf.Clamp01(normalizedTime * 2f));
            Vector3 dirToTarget = (target - lineStart).normalized;
            Vector3 rightAxis = Vector3.Cross(dirToTarget, Vector3.up);
            Vector3 upAxis = Vector3.Cross(dirToTarget, rightAxis);

            float totalLength = Vector3.Distance(lineStart, targetPos);
            float indexInterval = totalLength / (linePosCount - 2f);

            swingLine.SetPosition(0, lineStartPoint.position);
            swingLine.SetPosition(linePosCount - 1, targetPos);

            float freq = Mathf.Lerp(ropeBounceFrequency, 0f, normalizedTime);
            for (int i = 1; i < linePosCount - 1; i++)
            {
                float normalizedIndex = i / (linePosCount - 2f);
                float width = ropeBounceWidth * (1f - normalizedIndex);
                width *= Random.Range(0.8f, 1.2f);
                Vector3 pos = lineStart + dirToTarget * (indexInterval * i);
                pos += rightAxis * (Mathf.Sin(Time.time * ropeBounceSpeed +  i * freq) * width);
                pos += upAxis * (Mathf.Sin(-Time.time * ropeBounceSpeed + i * freq) * (width / 2f));
                swingLine.SetPosition(i, pos);
            }
            yield return null;
        }

        attackRopeRoutine = null;
        yield break;
    }

    //Swing methods
    private void HandleSwingState()
    {
        ropeLength = Mathf.Lerp(ropeLength, startRopeLength - swingRopeRemoveLength, Time.deltaTime * 5f);
        ropeLength = Mathf.Clamp(ropeLength, 5f, findSwingDistance);
        Vector3 center = swingPoint.transform.position;
        Vector3 dirToCenter = (center - transform.position).normalized;
        Vector3 start = transform.position;
        Vector3 newPosition = start + prevSwingMovement;

        Vector3 centerToNewPos = (newPosition - center).normalized;
        newPosition = center + centerToNewPos * ropeLength;

        Vector3 mov = newPosition - start;
        mov += inputManager.moveAxis * swingControlSpeed * Time.deltaTime;
        mov.y += swingGravity * Time.deltaTime;
        mov = Vector3.ClampMagnitude(mov, maxSwingMagnitude);
        controller.Move(mov * Time.deltaTime * swingSpeed);

        prevSwingMovement = mov;

        Quaternion targetRot = Quaternion.LookRotation(mov, center - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, swingRotateSpeed * Time.deltaTime);

        if (attackRopeRoutine == null)
        {
            int linePosCount = swingLine.positionCount;
            swingLine.SetPosition(0, lineStartPoint.position);
            swingLine.SetPosition(linePosCount - 1, swingPoint.transform.position);
            float totalLength = Vector3.Distance(lineStartPoint.position, center);
            float indexInterval = totalLength / (linePosCount - 2);
            for (int i = 1; i < linePosCount - 1; i++)
            {
                float normalizedIndex = Mathf.Clamp01(i / (linePosCount - 2f));
                float lerpSpeed = ropeLerpSpeed.Evaluate(normalizedIndex);
                Vector3 targetPos = lineStartPoint.position + dirToCenter * (indexInterval * i);
                Vector3 pos = Vector3.Lerp(swingLine.GetPosition(i), targetPos, Time.deltaTime * lerpSpeed);
                swingLine.SetPosition(i, pos);
            }
        }

        if (inputManager.buttonUp_swing)
        {
            if (ChangeMoveState(MoveState.Moving))
            {
                externalVelocity = mov * stopSwingForce;
                return;
            }
        }
    }
    private bool HandleStartSwinging()
    {
        swingPoint = SwingPoint.GetClosestPoint(transform.position + inputManager.moveAxis * 5f, findSwingDistance);
        RectTransform aim = ComponentManager<CanvasManager>.Value.aimSprite;
        aim.gameObject.SetActiveSafe(swingPoint != null);
        if (swingPoint == null)
            return false;

        aim.position = CanvasManager.WorldToScreenPoint(transform.position);

        if (inputManager.button_swing)
        {
            if (ChangeMoveState(MoveState.Swinging))
            {
                return true;
            }
        }
        return false;
    }

    //Glide methods
    private void HandleGlideState()
    {
        if (HandleStartSwinging())
        {
            return;
        }

        if (controller.isGrounded)
        {
            if (ChangeMoveState(MoveState.Moving))
            {
                return;
            }
        }

        HandleGravity(yVelocity > 0 ? gravity : glideGravity);
        if (yVelocity < minGlideYVelocity)
        {
            yVelocity = minGlideYVelocity;
        }

        HandleJump();


        //Add movement based on input
        if (inputManager.moveAxis != Vector3.zero)
        {
            //float currentAngle = Mathf.Atan2(glideDirection.z, glideDirection.x) * Mathf.Rad2Deg;
            //if (currentAngle < 0)
            //    currentAngle += 360f;
            //float targetAngle = Mathf.Atan2(inputManager.moveAxis.normalized.z, inputManager.moveAxis.normalized.x) * Mathf.Rad2Deg;
            //if (targetAngle < 0)
            //    targetAngle += 360f;
            //Debug.Log(targetAngle);
            //float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, glideTurnSpeed * Time.deltaTime);
            //glideDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * newAngle), 0f, Mathf.Sin(Mathf.Deg2Rad * newAngle));
            glideDirection = Vector3.Lerp(glideDirection, inputManager.moveAxis.normalized, glideTurnSpeed * Time.deltaTime);
        }

        float normalizedYVelocity = Mathf.Clamp01(Mathf.Abs(yVelocity / minGlideYVelocity));
        if (yVelocity > 0)
            normalizedYVelocity = 0f;
        float acceleration = Mathf.Lerp(minGlideAcceleration, maxGlideAcceleration, normalizedYVelocity);
        currentGlideSpeed = Mathf.Lerp(currentGlideSpeed, maxGlideSpeed, acceleration * Time.deltaTime);
        movement = glideDirection * currentGlideSpeed;
        movement.y = yVelocity;


        //Rotate
        Quaternion targetRotation = Quaternion.LookRotation(-groundHit.normal + transform.forward, movement);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * glideRotateSpeed);

        if (inputManager.buttonUp_glide)
        {
            if (ChangeMoveState(MoveState.Moving))
            {
                return;
            }
        }
    }
    private bool HandleStartGliding()
    {
        if (controller.isGrounded || groundedRay)
            return false;

        if (inputManager.buttonDown_glide)
        {
            if (ChangeMoveState(MoveState.Glide))
            {   
                return true;
            }
        }

        return false;
    }

    //Movement methods
    private void HandleGroundState()
    {
        if (HandleStartGliding())
        {
            return;
        }
        if (HandleStartSwinging())
        {
            return;
        }

        HandleGravity(gravity);
        HandleJump();
        HandleModelRotation();

        if (inputManager.buttonDown_dash)
        {
            Vector3 velocity = transform.forward.XZOnly().normalized * dashForce.z;
            velocity.y = dashForce.y;

            externalVelocity += velocity;
        }

        //Add movement based on input
        if (inputManager.moveAxis != Vector3.zero)
        {
            movement += inputManager.moveAxis.normalized;
        }

        float speed = inputManager.button_sprint ? sprintSpeed : movementSpeed;
        movement *= speed;
        movement.y = yVelocity;
    }
    private void HandleJump()
    {
        if (controller.isGrounded && AvailableJumps <= 0)
            AvailableJumps = 1;

        if (AvailableJumps > 0)
        {
            if (inputManager.buttonDown_jump)
            {
                if (controller.isGrounded)
                {
                    AvailableJumps++;
                }

                Jump();
            }
        }
    }
    private void Land(float velocity)
    {
        if (velocity >= 10f)
        {
            if (!landParticles.isPlaying)
                landParticles.Play();

            squisher.Squish(modelTransform, squish_Land);
        }
    }
    private void Jump()
    {
        landParticles.Play();
        yVelocity = jumpSpeed;
        AvailableJumps--;
    }
    private Vector3 GetCameraDirection()
    {
        Camera cam = Camera.main;
        Vector3 camDir = cam.transform.right + cam.transform.up + cam.transform.forward;
        camDir.y = 0f;

        return camDir;
    }

    //Bouncepad
    private void HandleBouncepad(ControllerColliderHit hit)
    {
        if (recentlyBounced)
            return;

        if (hit.moveDirection.y < 0 && yVelocity < -1)
        {
            recentlyBounced = true;
            Invoke("ResetRecentlyBounced", 0.5f);

            ChangeMoveState(MoveState.Moving);
            yVelocity *= -1;

            squisher.Squish(modelTransform, squish_Land);
            hit.transform.GetComponentInParent<Bouncepad>().PlayBounceEffect();
        }
    }
    private void ResetRecentlyBounced()
    {
        recentlyBounced = false;
    }

    //Pickups
    private void HandlePickup(ControllerColliderHit hit)
    {
        ComponentManager<GameManager>.Value.AddPickupAmount(1);

        ParticleSystem particles = hit.transform.GetComponentInChildren<ParticleSystem>();
        particles.transform.SetParentSafe(null);
        particles.Play();

        Destroy(hit.gameObject);
    }

}
