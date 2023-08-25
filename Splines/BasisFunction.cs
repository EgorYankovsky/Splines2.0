namespace Project;

public static class BasisFunction
{
    /// <summary>
    /// Базисная функция 1
    /// </summary>
    /// <param name="xi">Значение кси.</param>
    /// <returns>Значение функции в точке.</returns>
    private static double f1(double xi) => 1 - 3 * xi * xi + 2 * xi * xi * xi;

    /// <summary>
    /// Базисная функция 2
    /// </summary>
    /// <param name="xi">Значение кси.</param>
    /// <returns>Значение функции в точке.</returns>
    private static double f2(double xi) => xi - 2 * xi * xi + xi * xi * xi;

    /// <summary>
    /// Базисная функция 3
    /// </summary>
    /// <param name="xi">Значение кси.</param>
    /// <returns>Значение функции в точке.</returns>
    private static double f3(double xi) => 3 * xi * xi - 2 * xi * xi * xi;

    /// <summary>
    /// Базисная функция 4
    /// </summary>
    /// <param name="xi">Значение кси.</param>
    /// <returns>Значение функции в точке.</returns>
    private static double f4(double xi) => -1 * xi * xi + xi * xi * xi;

    /// <summary>
    /// Метод, возвращающий значение необходимой функции в точке x с шагом h.
    /// </summary>
    /// <param name="i">Порядковый номер базисной функции.</param>
    /// <param name="h">Шаг.</param>
    /// <param name="x">Точка.</param>
    /// <returns>Значение функции в точке.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Исключение на случай отсутствия порядкового номера базисной функции.</exception>
    public static double GetValue(int i, double h, double x)
    {
        return i switch
        {
            0 => f1(x),
            1 => h * f2(x),
            2 => f3(x),
            3 => h * f4(x),
            _ => throw new ArgumentOutOfRangeException("No such basis function"),
        };
    }
}