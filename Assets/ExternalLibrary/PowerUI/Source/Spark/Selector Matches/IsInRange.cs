//--------------------------------------//               PowerUI////        For documentation or //    if you have any issues, visit//        powerUI.kulestar.com////    Copyright � 2013 Kulestar Ltd//          www.kulestar.com//--------------------------------------using System;namespace Css{		/// <summary>	/// Describes if an element is in-range	/// <summary>	sealed class IsInRange:CssKeyword{				public override string Name{			get{				return "in-range";			}		}				public override SelectorMatcher GetSelectorMatcher(){			return new InRangeMatcher();		}			}		/// <summary>	/// Handles the matching process for :in-range.	/// </summary>		sealed class InRangeMatcher:LocalMatcher{				public override bool TryMatch(Dom.Node node){						if(node==null){				return false;			}						// Get the value:			int value;			if(int.TryParse(node.getAttribute("value"),out value)){								// Get min:				int min;				int.TryParse(node.getAttribute("min"),out min);								if(value<min){					return false;				}								// Get max:				int max;				if(!int.TryParse(node.getAttribute("max"),out max)){					max=int.MaxValue;				}								if(value>max){					return false;				}								// In range!				return true;							}						return false;		}			}	}