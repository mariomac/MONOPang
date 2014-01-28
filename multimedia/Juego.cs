/*------------------------------------------------------------------------------
 * Este código está distribuido bajo una licencia del tipo BEER-WARE.
 * -----------------------------------------------------------------------------
 * Mario Macías Lloret escribió este archivo. Teniendo esto en cuenta,
 * puedes hacer lo que quieras con él: modificarlo, redistribuirlo, venderlo,
 * etc, aunque siempre deberás indicar la autoría original en tu código.
 * Además, si algún día nos encontramos por la calle y piensas que este código
 * te ha sido de utilidad, estás obligado a invitarme a una cerveza (a ser
 * posible, de las buenas) como recompensa por mi contribución.
 * -----------------------------------------------------------------------------
 */

/**
 * Esta es la clase principal del juego. Tiene las siguientes funciones:
 * <ul>
 *   <li>Dibuja todos los elementos por pantalla (método draw)</li>
 *   <li>Llama al metodo mover() de todos los objetos con movimiento</li>
 *   <li>Gestiona (añade, elimina) los objetos móviles</li>
 *   <li>Muestra la presentación y el mensaje de fin de juego</li>
 *   <li>Lleva la cuenta de la puntuación del jugador</li>
 * </ul>
 * @author Mario Macías http://mario.site.ac.upc.edu
 */
using System;
using System.Collections.Generic;


public class Juego {
    /**
     * una constante que indica el tamaño en pixels (tanto ancho como alto) de
     * las paredes laterales y el suelo
     */
	private const int MARGEN = 24;

    /**
     * Lista que guarda todos los objetos animados: bolas, protagonista, disparos...
     */
    private List<ObjetoAnimado> objetosAnimados;

    /**
     * Instancia de la ventana donde se mostrará el juego
     */
    private Ventana ventana;

    /**
     * Tiempo (en milisegundos) entre bola y bola que aparece por el techo.
     */
    private long frecuenciaEntreBolas;
    /**
     * Tiempo (en milisegundos) en que apareció la última bola.
     */
    private long tiempoDeUltimaBola = 0;
    /**
     * Puntuación de la partida
     */
    private int puntuacion;

    /**
     * Inicialmente, 15 segundos entre una bola y otra. Se irá decrementando poco a poco
     * para hacer el juego más difícil.
     */
	private const long MINIMA_FRECUENCIA_ENTRE_BOLAS = 15000;
    /**
     * Para que el juego no acabe siendo imposible, la frecuencia máxima será de una
     * bola cada 5 segundos.
     */
	private const long MAXIMA_FRECUENCIA_ENTRE_BOLAS = 5000;
    /**
     * Por cada bola que salga, el tiempo que tarda entre ésta y la siguiente
     * Se acortará 200 milisegundos
     */
	private const long ACELERACION_FRECUENCIA_BOLAS = 200;

	private const Random RANDOM = new Random();

    /**
     * Será "true" cuando una bola haya tocado al jugador, y el juego haya acabado.
     */
    private bool finDeJuego;
    /**
     * Se limita a crear una nueva Ventana y asociarla al juego cuya escena
     * mostrará.
     */
	public Juego(Ventana v) {
		this.ventana = v; 
    }

    /**
     * Lo que es propiamente el juego, ocurre dentro de esta función.
     */
    public void partida() {
        //inicia algunos datos
        frecuenciaEntreBolas = MINIMA_FRECUENCIA_ENTRE_BOLAS;
        objetosAnimados = new ArrayList<ObjetoAnimado>();
        objetosAnimados.add(new Protagonista(this));
        tiempoDeUltimaBola = 0;
        finDeJuego = false;
        puntuacion = 0;
        Disparo.setTotalDisparos(0);

        // No saldrá de aquí mientras no hayan tocado al jugador (finDeJuego==true)
        while(!finDeJuego) {
            nuevoFotogramaDeJuego();
            ventana.mostrarLienzo();
        }
    }
    /**
     * Este método gestiona los cambios que se hacen en cada fotograma:
     * <li>Dibuja el escenario (techo, suelos...)</li>
     * <li>Llama a todos los elementos para que se muevan un pasito y se pinten
     *     en el lienzo</li>
     * <li>Si toca, lanza una nueva bola desde el techo</li>
     * <li>Una vez todo está pintado, sobreimpresiona la puntuación</li>
     */
    public void nuevoFotogramaDeJuego() {
        //Borra el lienzo, ya que todavía contiene el dibujo del fotograma anterior
        ventana.borrarLienzoOculto();

        //Dibujamos el techo y el suelo
        ventana.dibujaRectangulo(0,0,MARGEN,ventana.getAlturaLienzo(), Color.yellow);
        ventana.dibujaRectangulo(getCoordenadaXMargenDerecho(), 0, MARGEN, ventana.getAlturaLienzo(), Color.yellow);
        ventana.dibujaRectangulo(0, getCoordenadaYSuelo(), ventana.getAnchuraLienzo(), MARGEN, Color.yellow);

        //Mueve y pinta todos los objetos en pantalla
        //creamos una lista aparte para no tener error de modificación
        //en concurrencia cuando alguno de los objetos solicite insertar o borrar
        //elementos en la lista de objetos animados
        Iterator<ObjetoAnimado> iteradorObjetos = new LinkedList<ObjetoAnimado>(objetosAnimados).iterator();
        while(iteradorObjetos.hasNext()) {
            iteradorObjetos.next().moverYDibujar(ventana);
        }        

        //Mira si hay que lanzar una nueva bola desde el techo
        long ahora = System.currentTimeMillis();
        if(ahora > tiempoDeUltimaBola + frecuenciaEntreBolas) {
            objetosAnimados.add(new Bola(this, (float) (
                       getCoordenadaXMargenIzquierdo()
                       + RANDOM.nextInt((int)(getCoordenadaXMargenDerecho() - getCoordenadaXMargenIzquierdo())))));
            tiempoDeUltimaBola = ahora;
        }
        //antes de mostrar el lienzo del juego, sobreimpresiona la puntuación
        ventana.escribeTexto("Puntos: " + puntuacion, 30, 20, 18, Color.white);
    }

    /**
     * Retorna un array con todos los objetos animados que hay en ese momento.
     * @return
     */
    public ObjetoAnimado[] getObjetosAnimados() {
        return objetosAnimados.toArray(new ObjetoAnimado[objetosAnimados.size()]);
    }

    /**
     * Pide que se elimine un objeto animado (por ejemplo, cuando una bola pequeña
     * ha sido pinchada, se pide que se elimine.
     * @param obj Una referencia al objeto a eliminar.
     */
    public void eliminarObjetoAnimado(ObjetoAnimado obj) {
        objetosAnimados.remove(obj);
    }

    /**
     * Pide que se añada un objeto animado a la lista (por ejemplo, cuando el
     * jugador lanza un disparo, éste es un objeto animado que se añade a esta
     * lista para ser movido y dibujado junto con los otros)
     * @param obj Una referencia al objeto a añadir
     */
    public void anyadirObjetoAnimado(ObjetoAnimado obj) {
        objetosAnimados.add(obj);
    }

    /**
     * Incrementa la puntuación en 1. De esta manera damos un acceso limitado
     * de este dato a los demás objetos.
     */
    public void incrementaPuntuacion() {
        puntuacion++;
    }

    /**
     * Devuelve la coordenada X de la pared derecha, donde las bolas rebotan,
     * y que también impide el paso del jugador.
     * @return
     */
    public float getCoordenadaXMargenDerecho() {
        return ventana.getAnchuraLienzo() - MARGEN;
    }
    /**
     * Devuelve la coordenada X de la pared izquierda, donde las bolas rebotan,
     * y que también impide el paso del jugador.
     * @return
     */
    public float getCoordenadaXMargenIzquierdo() {
        return MARGEN;
    }
    /**
     * Devuelve la coordenada Y del suelo, donde las bolas rebotan, y que es
     * donde se apoya el jugador.
     * @return
     */
    public float getCoordenadaYSuelo() {
        return ventana.getAlturaLienzo() - MARGEN;
    }

    /**
     * Notifica al juego que el jugador ha sido tocado, para que éste actúe
     * en consecuencia (es decir, marque la partida como finalizada).
     */
    public void jugadorTocado() {
        finDeJuego = true;
    }

    /**
     * Muestra la pantalla de fin de juego.
     */
    public void finDeJuego() {
        ventana.escribeTexto("Fin de Juego!", 120,200, 64, Color.green);
        ventana.mostrarLienzo();
        //Hacemos que duerma unos tres segundos, para evitar que la pantalla
        //de fin de juego desaparezca demasiado rápido si el jugador
        //aprieta la barra espaciadora sin querer (porque, por ejemplo, estaba
        //disparando cuando le tocó la bola).
        try {
            Thread.sleep(3000);
        } catch (InterruptedException ex) {
            ex.printStackTrace();
        }

        while(!ventana.isPulsadoEspacio()) {
            // espera a que se pulse espacio para salir del mensaje de Fin de juego
        }
    }

    /**
     * Muestra la pantalla de presentación
     */
    public void presentacion() {
        ventana.borrarLienzoOculto();

        ventana.escribeTexto("MOO Pang!", 150,150, 64, Color.red);
        ventana.escribeTexto("Pulsa espacio para empezar", 180,420, 18, Color.white);
        ventana.mostrarLienzo();
        while(!ventana.isPulsadoEspacio()) {
            // espera a que se pulse espacio para salir de la presentacion
        }
    }
}
