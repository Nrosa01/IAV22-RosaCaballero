# Proyeco Final de IAV - Nicolás Rosa Caballero
## Parasite - IA Adaptativa en runtime

## Indice
- [Proyeco Final de IAV - Nicolás Rosa Caballero](#proyeco-final-de-iav---nicolás-rosa-caballero)
  - [Parasite - IA Adaptativa en runtime](#parasite---ia-adaptativa-en-runtime)
  - [Indice](#indice)
- [Descripción del proyecto](#descripción-del-proyecto)
- [Punto de partida](#punto-de-partida)
- [Paquetes de Unity añadidos](#paquetes-de-unity-añadidos)
- [Assets](#assets)
- [Funcioamiento de la IA](#funcioamiento-de-la-ia)
  - [Comportamiento de la IA](#comportamiento-de-la-ia)
- [Pseudocodigo](#pseudocodigo)
- [Controles](#controles)

# Descripción del proyecto

El proyecto consiste en un prototipo de un juego de acción en tercera persona con elemento de shooters "twin-stick" y un ritmo rápido fuertemente inspirado en __No More Heroes__ y __Furi__. Al igual que este último este juego consiste en una boss rush en espacios limitados. El Gimmick de este juego recae en una mecánica e IA únicas. Por un lado, tanto nuestro personaje como el enemigo tienen el mismo set de acciones, moverse, ataque a melee, ataque de rango, habilidad de movimiento y habilidad especial, similar a una "ulti". Lo característico es que estas habilidades pueden cambiar en tiempo de ejecución, en concreto las del jugador. Este, al recibir daño es "corrompido" por el enemigo y sus habilidades cambian a otras similares a las de este. Por otro lado, el enemigo usará una IA adaptativa que reaccione a estos cambios y buscará patrones más óptimos para ganar.

La IA del enemigo además deberá permitir configuraciones de tendencias, podría ser más agresiva, más cauta... Estas "restricciones" forzarán  la IA a salir de la ruta más óptima para ganar a cambio de tener más personalidad a la vez que tratarán de compensar sus debilidades lo máximo posible. Cuando los ataques del jugador cambian, el rango y daño de estos pueden cambiar, lo cual también influye en la IA.

Por cuestiones técnicas y de tiempo no habrá obstáculos en el mapa, pero de haberlos la IA debería poder sortearlos sin necesidad de un navmesh usando sensores y la información que recibe de ellos.

# Punto de partida

Aunque la parte más relevante del proyecto sea la IA del enemigo, he considero crear un proyecto base del juego con una arquitectura decente para facilitarme la tarea posterior. El punto de partida es un proyecto vacío de Unity 2021.2.10f1 con los paquetes que menciono en el apartado siguiente. Tras eso he creado un pequeño sistema modular de acciones que funciona tanto para el jugado como el enemigo, además de una cámara que trata de mantener a ambos personajes visibles al mismo tiempo que trata de estar cerca. El funcioamiento de la cámara es similar al de __Furi__. El sistema de acciones ha sido implementado por mi desde 0, es flexible, soporta cooldown, es cancelable (una acción puede ser interrumpida, por ejemplo por recibir daño o por un QTE). Además también soporta acciones enlazadas, por ejemplo si tu ataque a melee tiene 3 partes, solo vas a ejecutar la 2 si ha pasado muy poco tiempo desde que se ejecutó la 1. Podría decirse que es un sistema de combos simplificado donde solo existe un combo por tipo de acción, pero lo considero suficiente para el alcance de este proyecto y del juego, más complejidad no es necesaria.

A nivel de gráficos uso un Shader Toon que realicé siguiendo un tutorial en YouTube, por lo que no estoy seguro de si cuenta como recurso propio. Como el juego tiene pocas luces y transcurre en un espacio pequeño he optado por realizarlo para el render en modo forward.

# Paquetes de Unity añadidos

Para la realización del proyecto, he importado los siguientes paquetes:

- Mathematics
  - Paquete de matemáticos necesario para usar mis librerías de matemáticas.
- TextMeshPro
  - Paquete para la creación de textos eficientes usado en la UI y menús.
- URP
  - Paquete usado para implementar una SRP con soporte de Shadergraph. Me facilita crear un apartado visual decente y ligero para prototipar mejor. Junto con este paquete instalé el de Post Processing.
- Input System
  - Me permite desacoplar el Input de las acciones del jugador y tener una mejor arquitectura.

- Unitask
  - Este es el único paquete que no es oficial de Unity. Uso para crear una tarea asíncrona en la que se ejecuta una función cada cierto tiempo GC friendly. Es una alternativa a las corrutinas de Unity muy solvente y que considero obligatoria en cualquier proyecto, hasta el punto de que es el primer paquete que añado.
- RioniUtils
  - Paquete propio que contiene una colección de scripts con métodos de extensión y funciones matemáticas que me ayudan a trabajar con los datos de Unity.

# Assets

La idea es usar el menor número de assets externos. Actualmente todos los assets que hay en el proyecto, incluidos materiales, shaders y texturas son realizados por mi. Como excepción hay un modelo de un Riolu (Pokemon) que estoy usando de forma temporal como personaje, ya que el modelo era compatible con las animaciones de mixamo. Ver las animaciones es un reflejo del estado del personaje, lo cual me ayuda a depurar el funcionamiento del prototipo.

Las texturas usadas (salvo las de Riolu) han sido realizadas bien en material maker (software gratuito de itchio), bien en medibang (programa de dibujo gratuito), bien bakeando en blender una textura procedural o bien pintando sobre la malla en substance painter (usando licencia de estudiante)

# Funcioamiento de la IA

En este apartdo voy a describir el funcioamineto de la IA del enemigo. Este apartado es similar al pseudocodigo que se mostrará en el siguiente, sin embargo en este apartado nos basaremos en el concepto de la IA de forma que cualquier persona pueda entender que es lo que hace, mientras que en el siguiente lo que veremos será un prototipo de posible implementación.

Lo primero que pensé que sería importante antes de modelar el comportamiento de la IA, es definir una serie de características o funciones que ha de cumplir, tras esto, ver que datos podría necesitar para poder realizarlo y por último relacionar estas funciones y datos para modelar el comportamiento.

- Características de la IA
  - Acercarse al jugador para atacar en caso de que vaya a atacar melee
  - Alejarse del jugador para evitar daño melee, atacar rango.
  - Usar habilidad de movimiento para esquivar ataques del jugador anticipándose a estos
  - Parametros para modificar su comportamiento (agresividad vs cautela)
  - Adaptarse en medio del combate a los cambios de habilidad del jugador
  - Bloquear ataques del jugador si es posible*
  - Usar su habilidad especial (signature) en el momento adecuado.
  - Adaptarse su patrón de comportamiento a la situación del combate (va ganando es más agresivo, va perdiendo es más cauto, por ejemplo)
  - La IA estará pensada para enfrentar a un solo enemigo, el jugador no puede invocar minions. En caso de disponer de suficinete tiempo esto podría cambiar.
  - La IA será consciente de los obstáculos del entorno y los usará para cubrirse (opcional, si hay suficiente tiempo)

* Como los ataques son configurables, puede que un proyectil del jugador se anule con uno del enemigo, la IA no sabrá esto de primeras, tendrá que aprenderlo durante el combate y adaptarse si esto cambia. También puede ser posible que el ataque a melee del enemigo y del jugador se contraarresten, o que el jugador pueda bloquear los proyectiles del enemigo con su ataque melee. La IA debe aprender que puede hacer o no el jugador respecto sus ataques durante el combate, aunque esto no implica que no pueda recibir información privilegiada de un observer*2.

* 2- Es posible que exista un observer, que seria un "segundo jugador" responsable de controlar al enemigo y darle órdenes o información. Esto no será algo definitivo hasta que no lo haya probado. Puede que este concepto funcione bien o puede que no.  

- Datos que va a necesitar la IA
  - Referencia al jugador
  - Información del rango de sus ataques (sería absurdo que tenga que aprender como van sus propios ataques)
  - Información de su hablidad de movimiento (distancia que cubre, duración, si le da invulnerabilidad, etc)
  - Información de su habilidad signatura (rango, si es de daño o de crowd control, etc)

Una vez tenemos esto definido, podemos modelar la IA.

## Comportamiento de la IA

#  Pseudocodigo

El pseudocodigo de este proyecto puede llegar a ser extenso, por lo cual lo he dividido por partes para facilitar la lectura. Estas partes están en archivos separados a los que se accede a través de los links que pondré a continuación. Cabe mencionar que el estilo de Pseudocodigo que he elegido es uno propio, tratando de que se parezca a C# y usando su nomenclatura. Considero que si el proyecto va a ser escrito en C#, hacer que el pseudocodigo sea similar y use características propias del lenguaje facilita la integración. El pseudocodigo tendrá anotaciones en Español, pero los comentarios del código en sí serán en inglés, ya que me gusta desarrollar en inglés.

 - [Pseudocodigo del buffer de acciones](ActionSystemPseudo.md)
 - [Pseudocodigo del sistema de IA](IASystemPseudo.md)

# Controles

En este apartado se describirá como se controla el juego. Cabe destacar que aunque el juego soporta gamepad o teclado y ratón, es recomendable jugarlo con mando ya que ha sido diseñado para ser jugado con con un gamepad, además, si la disposición de botones es tipo XBOX es aún más cómod que si la disposición es tipo PS4/PS5.

