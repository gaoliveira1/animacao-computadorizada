using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(AudioSource))]
public class meshDeformer : MonoBehaviour
{

    Mesh mesh;
    public Vector3[] verticesIniciais, verticesRuntime;
    public Vector3[] velocidadeVertices;
    public float forcaOffset = 0.1f;
    public float efeitoMola = 20f;
    public float damping = 5f;
    public Vector3 pontoReferencia;
    public AudioSource music1;
    public AudioSource music2;
    public AudioSource music3;
    Vector3 a, b;

    public AudioSource activeMusic;


    public float force = 10f;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        verticesIniciais = mesh.vertices;
        verticesRuntime = new Vector3[verticesIniciais.Length];
        for (int i = 0; i < verticesIniciais.Length; i++)
        {
            verticesRuntime[i] = verticesIniciais[i];
        }
        velocidadeVertices = new Vector3[verticesIniciais.Length];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        for (int i = 0; i < verticesRuntime.Length; i++)
        {
            AtualizaVertice(i);
        }
        mesh.vertices = verticesRuntime;
        mesh.RecalculateNormals();

        if (this.activeMusic.isPlaying)
        {
            float[] spectrum = new float[256];

            AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

            for (int i = 1; i < spectrum.Length - 1; i++)
            {
                a = Vector3.Cross(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0));
                b = Vector3.Cross(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2));
            }
            this.AddDeformacao(pontoReferencia, (a.magnitude * b.magnitude) / force);
        }

    }

    public void AddDeformacao(Vector3 ponto, float forca)
    {
        for (int i = 0; i < verticesRuntime.Length; i++)
        {
            AddForcaNoVertice(i, ponto, forca);
        }
    }

    void AddForcaNoVertice(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = verticesRuntime[i] - point;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        velocidadeVertices[i] += pointToVertex.normalized * velocity;
    }

    void AtualizaVertice(int i)
    {
        Vector3 velocity = velocidadeVertices[i];
        Vector3 displacement = verticesRuntime[i] - verticesIniciais[i];
        velocity -= displacement * efeitoMola * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        velocidadeVertices[i] = velocity;
        verticesRuntime[i] += velocity * Time.deltaTime;
    }

    public void musica1()
    {
        this.activeMusic.Stop();
        this.activeMusic = music1;
        this.activeMusic.Play();
    }

    public void musica2()
    {
        this.activeMusic.Stop();
        this.activeMusic = music2;
        this.activeMusic.Play();
    }

    public void musica3()
    {
        this.activeMusic.Stop();
        this.activeMusic = music3;
        this.activeMusic.Play();
    }

    public void stopMusic()
    {
        this.activeMusic.Stop();
    }
}
