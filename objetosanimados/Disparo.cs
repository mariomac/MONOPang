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
 * Este clase representa lo que el jugador puede disparar: los ganchos atados
 * a una cuerda.
 *
 * Mario Macías: http://mario.site.ac.upc.edu
 */
using System.Drawing;
using System;


public class Disparo : ObjetoAnimado {
    /**
     * Altura del triangulo que representa el gancho (en pixels)
     */
    private const float ALTURA_GANCHO = 24;
    /**
     * Anchura de la base del triangulo que representa el gancho (en pixels)
     */
    private const float BASE_FLECHA = 16;

    /**
     * Velocidad a la que se mueve el gancho (en pixels/fotograma)
     */
    private const float VELOCIDAD = 10;
    /**
     * Grosor de la cuerda que va atada al gancho
     */
    private const float GROSOR_CUERDA = 6;

    /**
     * Constante que indica cuántos disparos se pueden haber a la vez en
     * pantalla (lo limitamos para hacer el juego más dificil)
     */
    public const int MAXIMO_DISPAROS_SIMULTANEOS = 2;

    /**
     * Contador estático que cuenta cuántos ganchos hay en todo momento
     * en pantalla.
     */
    private static int disparosSimultaneos = 0;

    /**
     * Posicion x e y de la punta del gancho.
     */
    private float posicionY, posicionX;
    private Juego elJuego;

    /**
     * Crea un gancho, cuya altura empezará en el suelo.
     * @param elJuego Instancia del Juego en el que será lanzado.
     * @param posicionX Coordenada X en pixels del gancho.
     */
    public Disparo(Juego elJuego, float posicionX) {
        this.elJuego = elJuego;
        this.posicionX = posicionX;
        this.posicionY = elJuego.getCoordenadaYSuelo();
        disparosSimultaneos++;
    }

    /**
     * Modifica el contador de disparos simultáneos que hay en pantalla.
     * @param disparos Numero de disparos simultaneos contados.
     */
    public static void setTotalDisparos(int disparos) {
        disparosSimultaneos = disparos;
    }
    /**
     * Obtiene el valor del contador de disparos simultáneos que hay en pantalla.
     * @return Numero de disparos simultáneos contados.
     */
    public static int getTotalDisparos() {
        return disparosSimultaneos;
    }

    /**
     * Constante que representa el color marrón según sus valores rojo,verde,azul
     */
    private static readonly Color MARRON = Color.FromArgb(179, 89, 0);

    /**
     * Implementación del método moverYDibujar de la interfaz ObjetoAnimado.
     * Incrementa la altura de la flecha hasta que llega al techo. También
     * verifica si ha colisionado con alguna bola.
     * @param ventana La ventana donde se dibujará.
     */
    public void moverYDibujar(Ventana ventana) {
        //si la flecha ha tocado el techo, desapareces
        if(posicionY < 0) {
            elJuego.eliminarObjetoAnimado(this);
            disparosSimultaneos--;
        } else {
            // Si no, sigue subiendo
            posicionY -= VELOCIDAD;
        }

        //Comprobamos si colisiona con alguna bola de la lista del juego
        foreach(ObjetoAnimado obj in elJuego.getObjetosAnimados()) {

            if(obj is Bola && compruebaColision((Bola)obj)) {
                ((Bola)obj).pinchar();
                elJuego.eliminarObjetoAnimado(this);
                disparosSimultaneos--;
                break;
            }
        }

        //dibujamos una flecha y una cuerda
        ventana.dibujaTriangulo(posicionX, posicionY,
                posicionX - BASE_FLECHA / 2, posicionY + ALTURA_GANCHO,
                posicionX + BASE_FLECHA / 2, posicionY + ALTURA_GANCHO,
                Color.LightGray);
        ventana.dibujaRectangulo(posicionX - GROSOR_CUERDA / 2, posicionY + ALTURA_GANCHO,
                GROSOR_CUERDA, elJuego.getCoordenadaYSuelo()- (posicionY + ALTURA_GANCHO), MARRON);
    }

    /**
     * Comprueba si el gancho ha colisionado con una bola. Es decir, si la distancia
     * al centro de la bola es menor que su radio para alguno de los puntos del gancho
     * @param b La instancia de Bola cuya colisión queremos comparar
     * @return true si ha colisionado. false en caso contrario
     */
    public bool compruebaColision(Bola b) {
        //comprobamos si la bola colisiona con la cuerda
        if(Math.Abs(b.getCentroX() - posicionX) < Math.Abs(b.getRadio() + GROSOR_CUERDA / 2)
           && b.getCentroY() - b.getRadio() >= posicionY) {
            return true;
        }
        // si no ha colisionado con la cuerda, miramos si ha tocado algunas de las esquinas del gancho
        float esquinaDerechaDistanciaX = (posicionX + GROSOR_CUERDA / 2) - b.getCentroX();
        float esquinaIzquierdaDistanciaX = (posicionX - GROSOR_CUERDA / 2) - b.getCentroX();
        float esquinaDistanciaY = (posicionY + ALTURA_GANCHO) - b.getCentroY();
        if(Math.Sqrt(esquinaDerechaDistanciaX*esquinaDerechaDistanciaX+esquinaDistanciaY*esquinaDistanciaY) < b.getRadio()
            || Math.Sqrt(esquinaIzquierdaDistanciaX*esquinaIzquierdaDistanciaX+esquinaDistanciaY*esquinaDistanciaY) < b.getRadio()) {
            return true;
        }
        return false;
    }
}
