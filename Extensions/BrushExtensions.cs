using System.Windows.Media;

namespace DownTube.Extensions;

public static class BrushExtensions {

    public static Color WithA( this Color Col, byte A ) => new Color { R = Col.R, G = Col.G, B = Col.B, A = A };
    public static Color WithR( this Color Col, byte R ) => new Color { R = R, G = Col.G, B = Col.B, A = Col.A };
    public static Color WithG( this Color Col, byte G ) => new Color { R = Col.R, G = G, B = Col.B, A = Col.A };
    public static Color WithB( this Color Col, byte B ) => new Color { R = Col.R, G = Col.G, B = B, A = Col.A };
    public static Color With( this Color Col, byte? A = null, byte? R = null, byte? G = null, byte? B = null ) => new Color { R = R ?? Col.R, G = G ?? Col.G, B = B ?? Col.B, A = A ?? Col.A };

    public static void SetA( this ref Color Col, byte A ) => Col.A = A;
    public static void SetR( this ref Color Col, byte R ) => Col.R = R;
    public static void SetG( this ref Color Col, byte G ) => Col.G = G;
    public static void SetB( this ref Color Col, byte B ) => Col.B = B;

    public static void Set( this ref Color Col, byte? A = null, byte? R = null, byte? G = null, byte? B = null ) {
        if ( A is { } Alpha ) { Col.A = Alpha; }
        if ( R is { } Red   ) { Col.R = Red;   }
        if ( G is { } Green ) { Col.G = Green; }
        if ( B is { } Blue  ) { Col.B = Blue;  }
    }

    public static SolidColorBrush WithA( this SolidColorBrush Brush, byte A ) => new SolidColorBrush(Brush.Color.WithA(A));
    public static SolidColorBrush WithR( this SolidColorBrush Brush, byte R ) => new SolidColorBrush(Brush.Color.WithR(R));
    public static SolidColorBrush WithG( this SolidColorBrush Brush, byte G ) => new SolidColorBrush(Brush.Color.WithG(G));
    public static SolidColorBrush WithB( this SolidColorBrush Brush, byte B ) => new SolidColorBrush(Brush.Color.WithB(B));
    public static SolidColorBrush With( this SolidColorBrush Brush, byte? A = null, byte? R = null, byte? G = null, byte? B = null ) => new SolidColorBrush(Brush.Color.With(A, R, G, B));

    public static void SetA( this SolidColorBrush Brush, byte A ) => Brush.Color = Brush.Color.WithA(A);
    public static void SetR( this SolidColorBrush Brush, byte R ) => Brush.Color = Brush.Color.WithR(R);
    public static void SetG( this SolidColorBrush Brush, byte G ) => Brush.Color = Brush.Color.WithG(G);
    public static void SetB( this SolidColorBrush Brush, byte B ) => Brush.Color = Brush.Color.WithB(B);
    public static void Set( this SolidColorBrush Brush, byte? A = null, byte? R = null, byte? G = null, byte? B = null ) => Brush.Color = Brush.Color.With(A, R, G, B);

}