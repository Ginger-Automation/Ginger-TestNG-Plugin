package com.Calculator;
import org.testng.Assert;
import org.testng.annotations.Parameters;
import org.testng.annotations.Test;

public class CalculatorTests 
{
   
   @Test
   @Parameters({"Num1", "Num2"})
   public void testSum(int Num1, int Num2)
   {      	
      Assert.assertEquals(Calculator.doSum(Num1, Num2), Num1+Num2);
   }

   @Test
   @Parameters({"Num1", "Num2"})
   public void testMoltiple(int Num1, int Num2)
   {      
      Assert.assertEquals(Calculator.doMultipliy(Num1, Num2), Num1*Num2);
   }
}
