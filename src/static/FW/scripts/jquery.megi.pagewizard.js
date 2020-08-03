//页面向导
var Pagewizard = (function () {
	function Pagewizard(steps) {
		this.steps = steps;
	};
	//初始化
	Pagewizard.prototype.init= function (steps , selector) {
		if (steps != undefined) {
			this.steps = steps;
		}

		this.initWizard(selector);
	};
	//向导初始化
	Pagewizard.prototype.initWizard = function (selector) {

		if (selector == undefined) {
			selector = "body";
		}

		$(selector).pagewalkthrough({
			name: 'introduction',
			steps: this.steps
		});

		$(selector).pagewalkthrough('show');
	}
	return Pagewizard;
}());