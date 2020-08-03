using System;
using System.Collections;

namespace JieNor.Megi.Common
{
	public class CalcParenthesesExpression
	{
		public string CalculateParenthesesExpression(string Expression)
		{
			ArrayList operatorList = new ArrayList();
			string ExpressionString2 = "";
			Expression = Expression.Replace(" ", "");
			string operand6;
			while (Expression.Length > 0)
			{
				operand6 = "";
				char c;
				if (char.IsNumber(Expression[0]))
				{
					while (char.IsNumber(Expression[0]))
					{
						string str = operand6;
						c = Expression[0];
						operand6 = str + c.ToString();
						Expression = Expression.Substring(1);
						if (Expression == "")
						{
							break;
						}
					}
					ExpressionString2 = ExpressionString2 + operand6 + "|";
				}
				int num;
				if (Expression.Length > 0)
				{
					c = Expression[0];
					num = ((c.ToString() == "(") ? 1 : 0);
				}
				else
				{
					num = 0;
				}
				if (num != 0)
				{
					operatorList.Add("(");
					Expression = Expression.Substring(1);
				}
				operand6 = "";
				int num2;
				if (Expression.Length > 0)
				{
					c = Expression[0];
					num2 = ((c.ToString() == ")") ? 1 : 0);
				}
				else
				{
					num2 = 0;
				}
				if (num2 != 0)
				{
					while (true)
					{
						if (!(operatorList[operatorList.Count - 1].ToString() != "("))
						{
							break;
						}
						operand6 = operand6 + operatorList[operatorList.Count - 1].ToString() + "|";
						operatorList.RemoveAt(operatorList.Count - 1);
						//bool flag = true;
					}
					operatorList.RemoveAt(operatorList.Count - 1);
					ExpressionString2 += operand6;
					Expression = Expression.Substring(1);
				}
				operand6 = "";
				int num3;
				if (Expression.Length > 0)
				{
					c = Expression[0];
					if (!(c.ToString() == "*"))
					{
						c = Expression[0];
						if (!(c.ToString() == "/"))
						{
							c = Expression[0];
							if (!(c.ToString() == "+"))
							{
								c = Expression[0];
								num3 = ((c.ToString() == "-") ? 1 : 0);
								goto IL_0211;
							}
						}
					}
					num3 = 1;
				}
				else
				{
					num3 = 0;
				}
				goto IL_0211;
				IL_0211:
				if (num3 != 0)
				{
					c = Expression[0];
					string @operator = c.ToString();
					if (operatorList.Count > 0)
					{
						if (operatorList[operatorList.Count - 1].ToString() == "(" || verifyOperatorPriority(@operator, operatorList[operatorList.Count - 1].ToString()))
						{
							operatorList.Add(@operator);
						}
						else
						{
							operand6 = operand6 + operatorList[operatorList.Count - 1].ToString() + "|";
							operatorList.RemoveAt(operatorList.Count - 1);
							operatorList.Add(@operator);
							ExpressionString2 += operand6;
						}
					}
					else
					{
						operatorList.Add(@operator);
					}
					Expression = Expression.Substring(1);
				}
			}
			operand6 = "";
			while (operatorList.Count != 0)
			{
				operand6 = operand6 + operatorList[operatorList.Count - 1].ToString() + "|";
				operatorList.RemoveAt(operatorList.Count - 1);
			}
			ExpressionString2 += operand6.Substring(0, operand6.Length - 1);
			return CalculateParenthesesExpressionEx(ExpressionString2);
		}

		private string CalculateParenthesesExpressionEx(string Expression)
		{
			ArrayList operandList = new ArrayList();
			Expression = Expression.Replace(" ", "");
			string[] operand3 = Expression.Split(Convert.ToChar("|"));
			for (int i = 0; i < operand3.Length; i++)
			{
				if (char.IsNumber(operand3[i], 0))
				{
					operandList.Add(operand3[i].ToString());
				}
				else
				{
					float operand2 = (float)Convert.ToDouble(operandList[operandList.Count - 1]);
					operandList.RemoveAt(operandList.Count - 1);
					float operand = (float)Convert.ToDouble(operandList[operandList.Count - 1]);
					operandList.RemoveAt(operandList.Count - 1);
					operandList.Add(calculate(operand, operand2, operand3[i]).ToString());
				}
			}
			return operandList[0].ToString();
		}

		private bool verifyOperatorPriority(string Operator1, string Operator2)
		{
			if (Operator1 == "*" && Operator2 == "+")
			{
				return true;
			}
			if (Operator1 == "*" && Operator2 == "-")
			{
				return true;
			}
			if (Operator1 == "/" && Operator2 == "+")
			{
				return true;
			}
			if (Operator1 == "/" && Operator2 == "-")
			{
				return true;
			}
			return false;
		}

		private float calculate(float operand1, float operand2, string operator2)
		{
			switch (operator2)
			{
			case "*":
				operand1 *= operand2;
				break;
			case "/":
				operand1 /= operand2;
				break;
			case "+":
				operand1 += operand2;
				break;
			case "-":
				operand1 -= operand2;
				break;
			}
			return operand1;
		}
	}
}
