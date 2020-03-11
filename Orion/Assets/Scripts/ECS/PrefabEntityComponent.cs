
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PrefabEntityComponent : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public static Entity prefabEntity;

    public GameObject prefabGameObject;

    public void Convert(Entity entity, EntityManager leManager, GameObjectConversionSystem conversionSystem)
    {
        // conversion de notre prefab game object en entity
        Entity prefabConvertiEntity = conversionSystem.GetPrimaryEntity(prefabGameObject);

        // On stock ensuite le prefab converti en entity dans la variable static prefabEntity pour pouvoir y acceder de n'imoporte où
        PrefabEntityComponent.prefabEntity = prefabConvertiEntity;
    }


    // il semblerait qu'un prefab doive faire parti de la liste referencedPrefabs pour être converti 
    // No sabemos porque lol
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        //On y ajoute donc notre prefab au format gameObject
        referencedPrefabs.Add(prefabGameObject);
    }
}
