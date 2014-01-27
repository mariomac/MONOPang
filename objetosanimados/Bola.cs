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

package moo.pang.objetosanimados;

import java.awt.Color;
import moo.multimedia.Ventana;
import moo.pang.multimedia.Juego;

/**
 * Clase que implementa las bolas que botan por la pantalla.
 * @author Mario Macías: http://mario.site.ac.upc.edu
 */
public class Bola implements ObjetoAnimado {
    /**
     * Aceleración de la bola hacia el suelo. Simula la fuerza de la gravedad.
     * Su valor no está en el sistema métrico, sino en una métrica nuestra:
     * pixels /(fotograma*fotograma).
     */
    protected static final float ACELERACION = 1.0f;

    /**
     * Velocidad horizontal de las bolas. Su valor no está en el sistema métrico,
     * sino en pixels/fotograma.
     */
    public static final float VELOCIDAD_HORIZONTAL = 4;

    /**
     * Velocidad máxima que tendrá una bola al rebotar (la más grande). Su valor
     * no está en sistema métrico sino en pixels/fotograma.
     */
    protected static final float VELOCIDAD_MAXIMA_REBOTE = -25;
    /**
     * Radio máximo que puede tener una bola, en píxels.
     */
    protected static final float RADIO_MAXIMO = 50;

    /**
     * Posición del centro de la bola.
     */
    protected float centroX, centroY; //posición de su centro
    /**
     * Radio de la bola.
     */
    protected float radio; //radio de la bola
    /**
     * Velocidad vertical de la bola (variable, porque tiene aceleración)
     */
    protected float velocidadY;
    /**
     * Velocidad horizontal de la bola (constante, de +VELOCIDAD_HORIZONTAL o
     * -VELOCIDAD_HORIZONTAL, según si va hacia la derecha o la izquierda.
     */
    protected float velocidadX;

    /**
     * Referencia al objeto Juego, para llamar a algunos de sus métodos.
     */
    private Juego elJuego;
    /**
     * Crea una bola que rebotará en los márgenes de una pantalla dada
     * @param pantalla La pantalla cuyos márgenes se deben tener en cuenta
     */
    public Bola(Juego elJuego, float posicionXInicial) {
        this.elJuego = elJuego;
        radio = RADIO_MAXIMO;
        centroX = posicionXInicial;
        centroY = -RADIO_MAXIMO;
        velocidadY = 0;
        velocidadX =  centroX < elJuego.getCoordenadaXMargenDerecho()/2 ? VELOCIDAD_HORIZONTAL : -VELOCIDAD_HORIZONTAL; //
    }

    //Las tres tonalidades de la bola en formato RED-GREEN-BLUE
    private static final Color SOMBRA = new Color(0.6f,0f,0f);
    private static final Color CUERPO = new Color(0.9f,0f,0f);
    private static final Color BRILLO = new Color(1f,0.85f,0.85f);

    /**
     * Este método mueve un poco la bola según la física descrita en el código
     * contenido: aplica gravedad, mueve verticalmente y horizontalmente, la
     * hace rebotar en caso de que la bola toque el suelo o la pared... Y después
     * de moverla, la dibuja en la posición de pantalla que corresponda en el
     * siguiente fotograma.
     *
     * @param ventana Instancia de Ventana donde se debe dibujar la bola.
     */
    public void moverYDibujar(Ventana ventana) {
        // calculamos primero los márgenes con los que puede chocar la bola (paredes y suelo)
        float margenIzq = elJuego.getCoordenadaXMargenIzquierdo();
        float margenSuelo = elJuego.getCoordenadaYSuelo();
        float margenDer = elJuego.getCoordenadaXMargenDerecho();

        //Para permitir una mejor visión al jugador de por dónde aparecerán
        //las bolas: si la bola está entrando (su centro está fuera de la pantalla),
        //no se acelera con la gravedad y baja poquito a poco.
        if(centroY < 0) {
            centroY++;
        } else {
            // Si no, desplazamos el centro y aceleramos la velocidad vertical (gravedad)
            centroX += velocidadX; centroY += velocidadY;
            velocidadY += ACELERACION;
        }

        // si toca el suelo, hacemos que rebote hacia arriba (dando velocidad negativa)
        if(centroY + radio >= margenSuelo) {
            centroY = margenSuelo - radio; // también corregimos su posición para que no se sobreimponga al suelo
            velocidadY = VELOCIDAD_MAXIMA_REBOTE * (0.5f + 0.5f * radio / RADIO_MAXIMO);
        }
        //si toca uno de los márgenes laterales, hacemos que se invierta la velocidad horizontal
        if(velocidadX < 0 && centroX - radio <= margenIzq) {
            centroX = margenIzq + radio;
            velocidadX = -velocidadX;
        } else if(velocidadX > 0 && centroX + radio >= margenDer) {
            centroX = margenDer - radio;
            velocidadX = -velocidadX;
        }

        //pintamos 3 circulos de diferentes colores para dar sensacion de volumen
        ventana.dibujaCirculo(centroX, centroY, radio, SOMBRA);
        ventana.dibujaCirculo(centroX - radio/7f, centroY - radio/7f, radio*0.8f, CUERPO);
        ventana.dibujaCirculo(centroX - radio/2.5f, centroY - radio/2f, radio*0.15f, BRILLO);
    }

    /**
     * Indica el radio mínimo que debe tener una bola. Cuando una bola de un
     * radio menor o igual a este se pinche, ya no se dividirá en dos bolas
     * más pequeñas, sino que desaparecera.
     */
    private static final float RADIO_MINIMO = VELOCIDAD_HORIZONTAL * 2;

    /**
     * Porcentaje de reducción del radio de una bola al pincharse. En este caso,
     * cuando pinchemos una bola, se crearán dos nuevas bolas, cada una con un
     * radio equivalente al 60% de la bola anterior.
     */
    private static final float REDUCCION_RADIO = 0.6f;

    /**
     * Este método se llama cuando una bola es tocada por un gancho o cuerda
     * lanzada por el jugador.
     */
    public void pinchar() {
        //Reduce su tamaño, y crea otra bola igual, pero que va en otra direccion
        radio *= REDUCCION_RADIO;
        if(radio < RADIO_MINIMO) {
            //Si la bola es demasiado pequeña, no se crean dos bolas sino que
            //se elimina directamente
            elJuego.eliminarObjetoAnimado(this);
        } else {
            //Cuando una bola es tocada, le damos un pequeño empujón hacia arriba.
            velocidadY = VELOCIDAD_MAXIMA_REBOTE / 4;

            //Creamos una bola exactamente igual que la actual, pero con la
            //velocidad horizontal invertida (para que una vaya a cada lado).
            Bola clon = new Bola(elJuego, centroX - velocidadX);
            clon.centroY = centroY;
            clon.velocidadX = -velocidadX;
            clon.radio = radio;
            clon.velocidadY = velocidadY;
            elJuego.anyadirObjetoAnimado(clon);
            centroX += velocidadX;
        }
        elJuego.incrementaPuntuacion();
        //Además, añade un objeto Destello en el punto de impacto, para que el
        //jugador reciba una retroalimentación visual extra.
        elJuego.anyadirObjetoAnimado(new Destello(centroX,centroY, elJuego));
    }

    public float getCentroX() {
        return centroX;
    }
    public float getCentroY() {
        return centroY;
    }
    public float getRadio() {
        return radio;
    }
}
