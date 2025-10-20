using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
	private static readonly int Speed = Animator.StringToHash("speed");
	private static readonly int SpeedY = Animator.StringToHash("speedY");
	private static readonly int IsFalling = Animator.StringToHash("isFalling");

	public Animator playerAnimator;
	[Space]
	
    [SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]
	public UnityEvent OnLandEvent;

	private float movement = 0;
	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		OnLandEvent ??= new UnityEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circle cast to the ground check position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D overlap = Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		if (overlap && overlap.gameObject != gameObject)
		{
			m_Grounded = true;
			if (!wasGrounded)
				OnLandEvent.Invoke();
		}
		else m_Grounded = false;

		Move();
	}

	private void LateUpdate()
	{
		playerAnimator.SetFloat(Speed, Mathf.Abs(movement));
		playerAnimator.SetFloat(SpeedY, m_Rigidbody2D.linearVelocityY);
		playerAnimator.SetBool(IsFalling, !m_Grounded);
	}
	
	public void UpdateMovement(float move)
	{
		movement = move;
	}


	private void Move()
	{
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(movement * 10f, m_Rigidbody2D.linearVelocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			switch (movement)
			{
				case > 0 when !m_FacingRight:
				case < 0 when m_FacingRight:
					Flip();
					break;
			}
		}
	}

	public void JumpPlayer()
	{
		// If the player should jump...
		if (m_Grounded)
		{
			// Add a vertical force to the player.
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
