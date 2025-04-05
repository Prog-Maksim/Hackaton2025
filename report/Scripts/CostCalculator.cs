namespace report.Scripts;

/* Примечание: формулы
 
 1 цк: цена = тариф * мощность (квт.ч)
 
 2 цк: цена = тариф_ночь * мощность_ночь (квт.ч) + тариф_день * мощность_день (квт.ч)
 или цена = тариф_ночь * мощность_ночь (квт.ч) + тариф_пик * мощность_пик (квт.ч) + тариф_полупик * мощность_полупик (квт.ч)
 
 3 цк: цена = (тариф_час1 * электричество_час1 (квт.ч) + ...) + (эл_в_отчётный_час_день1 (квт.ч) + ...) / колво_дней * цена_мощности
 или добавляем тариф_по_передаче * колво_электричества
 
 4 цк: цена = (тариф_час1 * электричество_час1 (квт.ч) + ...) + (эл_в_отчётный_час_день1 (квт.ч) + ...) / колво_дней * цена_мощности
 или добавляем тариф_по_передаче * колво_электричества
*/

public static class CostCalculator
{
    /// <summary>
    /// расчёт общей стоимости по первой ценовой категории
    /// </summary>
    /// <param name="power">общая потреблённая мощность</param>
    /// <param name="tariff">стоимость тарифа</param>
    /// <returns>стоимость</returns>
    public static double PriceCategory1(double power, double tariff)
    {
        return tariff * power;
    }

    /// <summary>
    /// расчёт стоимости по первой ценовой категории за отдельные часы
    /// </summary>
    /// <param name="power">мощность по дням и часам, double[дни, часы]</param>
    /// <param name="tariff">стоимость тарифа</param>
    /// <returns>стоимость по дням и часам</returns>
    public static double[,] PriceCategory1(double[,] power, double tariff)
    {
        double[,] cost = new double[power.GetLength(0), power.GetLength(1)];

        for (int day = 0; day < cost.GetLength(0); day++)
        {
            for (int hour = 0; hour < cost.GetLength(1); hour++)
            {
                cost[day, hour] = power[day, hour] * tariff;
            }
        }

        return cost;
    }
    
    
    
    /// <summary>
    /// расчёт общей стоимости по второй ценовой категории день/ночь
    /// </summary>
    /// <param name="power">общая потреблённая мощность</param>
    /// <param name="currentHour">в какой час</param>
    /// <param name="twoZoneTariff">стоимость тарифа в ночное и дневное время, (ночная, двевная)</param>
    /// <param name="tariffHours">время действия зон, (до какого часа ночная, до какого часа дневная), по умолчанию (7, 23)</param>
    /// <returns>стоимость</returns>
    /// <exception cref="ArgumentException"></exception>
    public static double PriceCategory2(double power, byte currentHour, (double, double) twoZoneTariff, 
        (byte, byte)? tariffHours = null)
    {
        tariffHours ??= (7, 23);

        if (tariffHours.Value.Item1 > 23 || tariffHours.Value.Item2 > 23)
            throw new ArgumentException("Время в тарифе должно быть от 0 до 23 часов");

        if (currentHour < tariffHours.Value.Item1 || currentHour > tariffHours.Value.Item2)
            return power * twoZoneTariff.Item1;
        else
            return power * twoZoneTariff.Item2;
    }
    
    /// <summary>
    /// расчёт общей стоимости по второй ценовой категории пик/полупик/ночь
    /// </summary>
    /// <param name="power">общая потреблённая мощность</param>
    /// <param name="currentHour">в какой час</param>
    /// <param name="threeZoneTariff">стоимость тарифа в ночь, пик и полупик, (ночь, пик, полупик)</param>
    /// <param name="tariffHours">время действия зон – до какого часа, (ночь, пик1, полупик1, пик2, полупик2), по умолчанию (7, 10, 17, 21, 23)</param>
    /// <returns>стоимость</returns>
    /// <exception cref="ArgumentException"></exception>
    public static double PriceCategory2(double power, byte currentHour, (double, double, double) threeZoneTariff, 
        (byte, byte, byte, byte, byte)? tariffHours = null)
    {
        tariffHours ??= (7, 10, 17, 21, 23);

        if (tariffHours.Value.Item1 > 23 || tariffHours.Value.Item2 > 23 || tariffHours.Value.Item3 > 23
            || tariffHours.Value.Item4 > 23 || tariffHours.Value.Item5 > 23)
            throw new ArgumentException("Время в тарифе должно быть от 0 до 23 часов");

        if (currentHour < tariffHours.Value.Item1 || currentHour > tariffHours.Value.Item5)
            return power * threeZoneTariff.Item1;
        else if (currentHour < tariffHours.Value.Item2)
            return power * threeZoneTariff.Item2;
        else if (currentHour < tariffHours.Value.Item3)
            return power * threeZoneTariff.Item3;
        else if (currentHour < tariffHours.Value.Item2)
            return power * threeZoneTariff.Item2;
        else
            return power * threeZoneTariff.Item3;
    }

    /// <summary>
    /// расчёт стоимости по второй ценовой категории за отдельные часы день/ночь
    /// </summary>
    /// <param name="power">мощность по дням и часам, double[дни, часы]</param>
    /// <param name="twoZoneTariff">стоимость тарифа в ночное и дневное время, (ночная, двевная)</param>
    /// <param name="tariffHours">время действия зон, (до какого часа ночная, до какого часа дневная), по умолчанию (7, 23)</param>
    /// <returns>стоимость по дням и часам</returns>
    /// <exception cref="ArgumentException"></exception>
    public static double[,] PriceCategory2(double[,] power, (double, double) twoZoneTariff, 
        (byte, byte)? tariffHours = null)
    {
        tariffHours ??= (7, 23);

        if (tariffHours.Value.Item1 > 23 || tariffHours.Value.Item2 > 23)
            throw new ArgumentException("Время в тарифе должно быть от 0 до 23 часов");
        
        double[,] cost = new double[power.GetLength(0), power.GetLength(1)];

        for (int day = 0; day < cost.GetLength(0); day++)
        {
            for (int hour = 0; hour < cost.GetLength(1); hour++)
            {
                cost[day, hour] = power[day, hour] * (hour < tariffHours.Value.Item1 || hour > tariffHours.Value.Item2 ?
                    twoZoneTariff.Item1 : twoZoneTariff.Item2);
            }
        }

        return cost;
    }
    
    /// <summary>
    /// расчёт стоимости по второй ценовой категории за отдельные часы пик/полупик/ночь
    /// </summary>
    /// <param name="power">мощность по дням и часам, double[дни, часы]</param>
    /// <param name="threeZoneTariff">стоимость тарифа в ночь, пик и полупик, (ночь, пик, полупик)</param>
    /// <param name="tariffHours">время действия зон – до какого часа, (ночь, пик1, полупик1, пик2, полупик2), по умолчанию (7, 10, 17, 21, 23)</param>
    /// <returns>стоимость по дням и часам</returns>
    /// <exception cref="ArgumentException"></exception>
    public static double[,] PriceCategory2(double[,] power, (double, double, double) threeZoneTariff, 
        (byte, byte, byte, byte, byte)? tariffHours = null)
    {
        tariffHours ??= (7, 10, 17, 21, 23);

        if (tariffHours.Value.Item1 > 23 || tariffHours.Value.Item2 > 23 || tariffHours.Value.Item3 > 23
            || tariffHours.Value.Item4 > 23 || tariffHours.Value.Item5 > 23)
            throw new ArgumentException("Время в тарифе должно быть от 0 до 23 часов");
        
        double[,] cost = new double[power.GetLength(0), power.GetLength(1)];

        for (int day = 0; day < cost.GetLength(0); day++)
        {
            for (int hour = 0; hour < cost.GetLength(1); hour++)
            {
                if (hour < tariffHours.Value.Item1 || hour > tariffHours.Value.Item5)
                    cost[day, hour] = power[day, hour] * threeZoneTariff.Item1;
                else if (hour < tariffHours.Value.Item2)
                    cost[day, hour] = power[day, hour] * threeZoneTariff.Item2;
                else if (hour < tariffHours.Value.Item3)
                    cost[day, hour] = power[day, hour] * threeZoneTariff.Item3;
                else if (hour < tariffHours.Value.Item2)
                    cost[day, hour] = power[day, hour] * threeZoneTariff.Item2;
                else
                    cost[day, hour] = power[day, hour] * threeZoneTariff.Item3;
            }
        }

        return cost;
    }
    
    
    
    /// <summary>
    /// расчёт общей стоимости по третьей ценовой категории
    /// </summary>
    /// <param name="energy">общая потреблённая мощность</param>
    /// <param name="energyTariff">стоимость энергии за квт.ч</param>
    /// <param name="power">мощность в отчётный час</param>
    /// <param name="powerTariff">стоимость мощности</param>
    /// <param name="transmissionTariff">стоимость услуг передачи. по умолчанию 0 для контракта купли-продажи</param>
    /// <returns></returns>
    public static double PriceCategory3(double energy, double energyTariff, double power, double powerTariff, double transmissionTariff = 0)
    {
        return energy * energyTariff + power * powerTariff + transmissionTariff * energy;
    }

    // 3 цк: цена = (тариф_час1 * электричество_час1 (квт.ч) + ...) + (эл_в_отчётный_час_день1 (квт.ч) + ...) / колво_дней * цена_мощности
    // или добавляем тариф_по_передаче * колво_электричества
    public static double[,] PriceCategory3(double[,] energy, double[,] energyTariff, 
        int[] powerHours, double[] powerTariff, double[,]? transmissionTariff = null)
    {
        double[,] cost = new double[energy.GetLength(0), energy.GetLength(1)];
        double powerCost = 0, averagePowerTariff = 0;
        
        for (int day = 0; day < cost.GetLength(0); day++)
        {
            powerCost += energy[day, powerHours[day]];
            averagePowerTariff += powerTariff[day];
        }

        powerCost = powerCost / (powerHours.Length * powerHours.Length) * (averagePowerTariff / powerTariff.Length);

        for (int day = 0; day < cost.GetLength(0); day++)
        {
            for (int hour = 0; hour < cost.GetLength(1); hour++)
            {
                cost[day, hour] = energy[day, hour] * energyTariff[day, hour] + powerCost + 
                                  (transmissionTariff == null ? 0 : transmissionTariff[day, hour] * energy[day, hour]);
            }
        }

        return cost;
    }
}