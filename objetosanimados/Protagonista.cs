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
 * Implementa la clase que representa al personaje del juego.
 *
 * @author Mario Macías: http://mario.site.ac.upc.edu
 */
using System.Drawing;


public class Protagonista : ObjetoAnimado {
    /**
     * Posición x e y de la esquina superior izquierda del cuadro que engloba al protagonista.
     */
    private float posicionX, posicionY;

    private Juego elJuego;
    /**
     * Instancia al protagonista.
     * @param elJuego El objeto juego donde el protagonista estará presente.
     */
    public Protagonista(Juego elJuego) {
        this.elJuego = elJuego;
        posicionX = elJuego.getCoordenadaXMargenDerecho() / 2;
        posicionY = elJuego.getCoordenadaYSuelo() - Protagonista.ALTURA_COLISION;
    }

    /**
     * Implementa el método moverYDibujar de la interfaz ObjetoAnimado. Este método
     * comprueba qué teclas están pulsadas y mueve al jugador a izquierda o derecha
     * en consecuencia. Cuando la barra ha sido pulsada, también dispara un gancho.
     *
     * A continuación, comprueba si el jugador ha colisionado con alguna bola.
     *
     * Al final de todo, dibuja al protagonista en la ventana.
     *
     * @param ventana Ventana donde se dibujará el personaje.
     */
    public void moverYDibujar(Ventana ventana) {
        if(ventana.isPulsadoDerecha()) {
            posicionX += VELOCIDAD_HORIZONTAL;
            if(posicionX + ANCHURA_COLISION > elJuego.getCoordenadaXMargenDerecho()) {
                posicionX = elJuego.getCoordenadaXMargenDerecho() - ANCHURA_COLISION;
            }
        }
        if(ventana.isPulsadoIzquierda()) {
            posicionX -= VELOCIDAD_HORIZONTAL;
            if(posicionX < elJuego.getCoordenadaXMargenIzquierdo()) {
                posicionX = elJuego.getCoordenadaXMargenIzquierdo();
            }
        }
        if(ventana.isPulsadoEspacio() && Disparo.getTotalDisparos() < Disparo.MAXIMO_DISPAROS_SIMULTANEOS) {
            elJuego.anyadirObjetoAnimado(new Disparo(elJuego, posicionX + ANCHURA_COLISION / 2));
        }

        //Comprobar si alguna bola choca con el protagonista. Se define un rectangulo
        //que contiene la mayor parte del cuerpo del protagonista, cuyas esquina supe-
        //rior izquierda es el punto (posicionX,posicionY) y su esquina inferior derecha
        //es el punto (posicionX+ANCHURA_COLISION,posicionY+ALTURA_COLISION).
        //Si ese rectangulo
        //colisiona con alguna de las bolas, se le dice a la clase juego que el
        //jugador ha sido tocado, para que actue en consecuencia segun la funcion
        //jugadorTocado() de la clase Juego
        foreach(ObjetoAnimado obj in elJuego.getObjetosAnimados()) {
            if(obj is Bola) {
                Bola bola = (Bola) obj;
                if(bola.getCentroY() + bola.getRadio() >= posicionY) {
                    if((bola.getCentroX() + bola.getRadio() >= posicionX)
                        && (bola.getCentroX() - bola.getRadio() <= posicionX + ANCHURA_COLISION)) {
                        elJuego.jugadorTocado();
                        break;
                    }
                }
            }
        }

        float x = posicionX - MARGEN_IZQUIERDO_COLISION;
        //ventana.dibujaRectangulo(x+MARGEN_IZQUIERDO_COLISION, posicionY, ANCHURA_COLISION, ALTURA_COLISION, Color.white);
        ventana.dibujaCirculo(x+24, posicionY+16, 16, Color.Orange);
        ventana.dibujaCirculo(x+18, posicionY+14, 3, Color.Black);
        ventana.dibujaCirculo(x+30, posicionY+14, 3, Color.Black);
        ventana.dibujaRectangulo(x+16, posicionY+22, 16,3, Color.Black);
        ventana.dibujaTriangulo(x+24,posicionY+32,x+0,posicionY+44,x+4,posicionY+53, Color.Orange);
        ventana.dibujaTriangulo(x+24,posicionY+32,x+48,posicionY+44,x+44,posicionY+53, Color.Orange);
        ventana.dibujaRectangulo(x+16,posicionY+32,16,24,Color.White);
        ventana.dibujaRectangulo(x+14, posicionY+56, 8, 16, Color.Blue);
        ventana.dibujaRectangulo(x+26, posicionY+56, 8, 16, Color.Blue);
        ventana.dibujaRectangulo(x+10, posicionY+68, 12, 4, Color.Red);
        ventana.dibujaRectangulo(x+26, posicionY+68, 12, 4, Color.Red);
    }

    //Velocidad a la que se mueve el jugador
    private const float VELOCIDAD_HORIZONTAL = 6;
   
    private const float MARGEN_IZQUIERDO_COLISION = 12;
    private const float ANCHURA_COLISION = 24;
    private const float ALTURA_COLISION = 72;
}
