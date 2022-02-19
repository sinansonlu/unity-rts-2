using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnaOyun : MonoBehaviour
{
    public float[] kaynaklar;

    public Material mat_normal;
    public Material mat_secili;
    public Material mat_toplama;

    private GameObject mevcut_secili;

    public Bina[] kaynak_birakma_binalari;

    public void KaynakBirak(Birim b)
    {
        kaynaklar[b.toplanan_kaynak_cinsi] += b.eldeki_kaynak;
        b.eldeki_kaynak = 0;
    }

    public bool KaynakBirakacakNoktaBul(Birim b)
    {
        if(kaynak_birakma_binalari.Length == 0)
        {
            return false;
        }
        else
        {
            Vector3 birimKonumu = b.gameObject.transform.position;
            Vector3 enYakinKonum = kaynak_birakma_binalari[0].cikis_noktasi.transform.position;
            float enYakinMesafe = Vector3.Distance(birimKonumu,enYakinKonum);

            GameObject enYakinKonumObjesi = kaynak_birakma_binalari[0].cikis_noktasi;

            for (int i = 1; i < kaynak_birakma_binalari.Length; i++)
            {
                Vector3 simdikiKonum = kaynak_birakma_binalari[i].cikis_noktasi.transform.position;
                float simdikiMesafe = Vector3.Distance(birimKonumu, simdikiKonum);

                if(simdikiMesafe < enYakinMesafe)
                {
                    enYakinMesafe = simdikiMesafe;
                    enYakinKonum = simdikiKonum;
                    enYakinKonumObjesi = kaynak_birakma_binalari[i].cikis_noktasi;
                }
            }

            b.donus_noktasi = enYakinKonumObjesi;

            return true;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // seçim yapma
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f,LayerMask.GetMask("Birim","Bina")))
            {
                if(mevcut_secili != null)
                {
                    MeshRenderer mr_eski = mevcut_secili.GetComponent<MeshRenderer>();
                    if (mr_eski != null)
                    {
                        mr_eski.material = mat_normal;
                    }
                }

                mevcut_secili = hit.transform.gameObject;
                MeshRenderer mr = mevcut_secili.GetComponent<MeshRenderer>();
                if(mr != null)
                {
                    mr.material = mat_secili;
                }
             
            }
            else
            {
                if (mevcut_secili != null)
                {
                    MeshRenderer mr_eski = mevcut_secili.GetComponent<MeshRenderer>();
                    if (mr_eski != null)
                    {
                        mr_eski.material = mat_normal;
                    }
                    mevcut_secili = null;
                }
            }
        }
        // emir verme
        else if (Input.GetMouseButtonDown(1))
        {
            if(mevcut_secili != null)
            {
                LayerMask lm = LayerMask.GetMask("Birim");

                if (lm == (lm | (1 << mevcut_secili.layer)))
                {
                    // seçili olan þey bir birim

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    // kaynaða mý sað týkladýk?
                    if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Kaynak")))
                    {
                        Birim b = mevcut_secili.GetComponentInParent<Birim>();
                        if (b != null)
                        {
                            Kaynak k = hit.transform.gameObject.GetComponent<Kaynak>();
                            if(k != null)
                            {
                                // b birimi k kaynaðýný toplamalý
                                if(b.kaynak_toplayabilir)
                                {
                                    b.KaynagaGit(k);
                                }
                                else
                                {
                                    b.Yuru(hit.point);
                                }
                            }
                        }
                    }
                    else if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Zemin")))
                    {
                        Birim b = mevcut_secili.GetComponentInParent<Birim>();
                        if (b != null)
                        {
                            b.Yuru(hit.point);
                        }
                    }
                }

                lm = LayerMask.GetMask("Bina");

                if (lm == (lm | (1 << mevcut_secili.layer)))
                {
                    // seçili olan þey bir bina

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Zemin")))
                    {
                        Bina b = mevcut_secili.GetComponentInParent<Bina>();
                        if (b != null)
                        {
                            b.VarisSec(hit.point);
                        }
                    }
                }
            }
        }
        
    }
}
