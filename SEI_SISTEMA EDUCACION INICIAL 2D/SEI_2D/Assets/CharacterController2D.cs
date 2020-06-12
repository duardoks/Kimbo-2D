using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;
	//Cantidad de fuerza añadida cuando la jugadora salta
		[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;
	//Cantidad de velocidad máxima aplicada al movimiento agachado. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
	// Cuánto suavizar el movimiento
	[SerializeField] private bool m_AirControl = false;
	//Si un jugador puede dirigir o no mientras salta;
	[SerializeField] private LayerMask m_WhatIsGround;
	// Una máscara que determina lo que está molido para el personaje
	[SerializeField] private Transform m_GroundCheck;
	//Una posición que marca dónde verificar si el jugador está conectado a tierra
	[SerializeField] private Transform m_CeilingCheck;
	//Una posición que marca dónde verificar los techos
   [SerializeField] private Collider2D m_CrouchDisableCollider;				

	const float k_GroundedRadius = .2f;
	//Radio del círculo de superposición para determinar si está conectado a tierra
	private bool m_Grounded;
	// Si el jugador está conectado a tierra o no.
	const float k_CeilingRadius = .2f;
	// Radio del círculo de superposición para determinar si el jugador puede ponerse de pie
	private Rigidbody2D m_Rigidbody2D;

	// Para determinar hacia dónde se enfrenta el jugador actualmente.
	private bool m_FacingRight = true;  

	private Vector3 velocity = Vector3.zero;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}


	private void FixedUpdate()
	{
		m_Grounded = false;

		
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				m_Grounded = true;
		}
	}


	public void Move(float move, bool crouch, bool jump)
	{
		// Levantarse luego de chocar
		if (!crouch)
		{
			// 
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//Si Salta
		if (m_Grounded || m_AirControl)
		{

			// Si colisiona
			if (crouch)
			{
				
				move *= m_CrouchSpeed;

				// Deshabilitar Cronogramas
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Colisiones habilitadas durante salto
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;
			}

			// Mover personaje deacuerdo a la velocidad
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
	
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

			
			if (move > 0 && !m_FacingRight)
			{
			
				Flip();
			}
			
			else if (move < 0 && m_FacingRight)
			{
			
				Flip();
			}
		}
		
		if (m_Grounded && jump)
		{
			// Movimiento Vertical
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}


	private void Flip()
	{

		m_FacingRight = !m_FacingRight;


		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
