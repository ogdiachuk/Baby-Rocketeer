
using UnityEngine;
using UnityEngine.SceneManagement;

public class jetPack : MonoBehaviour
{

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] float effectsLoadDelay = 1f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainDeath;
    [SerializeField] AudioClip mainFinish;
    [SerializeField] AudioClip mainStart;
    [SerializeField] AudioClip Explode;

    [SerializeField] ParticleSystem ExplodeParticles;
    [SerializeField] ParticleSystem mainFinishParticles;
    [SerializeField] ParticleSystem JetsParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (state != State.Alive)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "finish":
                FinishingSequence();
                break;
            default:
                print("DEAD!"); // TODO remove
                DeathSequence();
                break;
        }
    }

    private void FinishingSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        mainFinishParticles.Play();
        audioSource.PlayOneShot(mainFinish);
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void DeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(mainDeath);
        Invoke("PlayExplode", effectsLoadDelay);
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void PlayExplode()
    {
        audioSource.PlayOneShot(Explode);
        ExplodeParticles.Play();

    }
    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StartEngine();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void StartEngine()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            JetsParticles.Play();
        }
    }

    private void RespondToRotateInput()
    {

        rigidBody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }
}



