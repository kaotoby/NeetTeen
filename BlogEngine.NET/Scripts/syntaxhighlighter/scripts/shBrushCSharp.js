;(function()
{
	// CommonJS
	SyntaxHighlighter = SyntaxHighlighter || (typeof require !== 'undefined'? require('shCore').SyntaxHighlighter : null);

	function Brush()
	{
		var keywords =	'abstract as base bool break byte case catch char checked class const ' +
						'continue decimal default delegate do double else enum event explicit volatile ' +
						'extern false finally fixed float for foreach get goto if implicit in int ' +
						'interface internal is lock long namespace new null object operator out ' +
						'override params private protected public readonly ref return sbyte sealed set ' +
						'short sizeof stackalloc static string struct switch this throw true try ' +
						'typeof uint ulong unchecked unsafe ushort using virtual void while var ' +
						'from group by into select let where orderby join on equals ascending descending';

		var classes1 =  'Array ArrayList Assembly Attribute AttributeUsage Boolean Byte Calendar Char Claim ClaimSet ' +
		                'Color Component Configuration Console CultureInfo DataColumn DataContext ' +
                        'DataContractJsonSerializer DataContractSerializer DataRow DataSet DataTable ' +
                        'DataView Debug Decimal Delegate DependencyObject Directory Dns Domain' +
                        'ECDsaCng Encoding EntitySet Enum Environment EventArgs EventDescriptor ' +
                        'Exception Expression FieldInfo File FileSecurity FileStream FtpWebRequest ' +
                        'GZipStream HashSet HashTable HttpListener HttpWebRequest Int32 Int64 List ' +
                        'Match Math Marshal MemberInfo';

		var classes2 =  'NetworkInterface NetworkStream Object OdbcCommand OdbcConnection OdbcDataAdapter OdbcDataReader ' +
                        'OleDbCommand OleDbConnection OleDbDataAdapter OleDbDataReader OperationDescription OracleCommand ' +
                        'OracleConnection OracleDataAdapter OracleDataReader Parallel Path ' +
                        'Process PropertyInfo Queryable Queue Regex ResourceManager';

		var classes3 =  'SerialPort ServiceBase ServicedComponent ServiceDescription ' +
                        'ServiceEndpoint ServiceHost ServiceHostFactory ServiceMoniker SHA1 SmtpClient ' +
                        'Socket SqlCommand SqlConnection SqlDataAdapter SqlDataReader SqlNotification SslStream Stack ' +
                        'Stream StreamReader StreamWriter String StringBuilder SyndicationFeed Table ' +
                        'Task TcpClient TcpListener TextInfo Thread TimeZoneInfo Trace Transaction ' +
                        'TripleDES Type TypeConverter UdpClient Uri';

        var classes4 =  'WebClient WebService WebServiceHost WindowsIdentity XAttribute XDocument ' +
                        'XElement XmlAttribute XmlDictionaryReader XmlDictionaryWriter XmlDocument XmlElement XmlMappingSource ' +
                        'XmlNode XmlReader XmlSchema XmlSchemaSet XmlSchemaValidator XmlSerializer XmlWriter XName XNamespace ' +
                        'XNode XPathDocument XPathExpression XPathNavigator XText';

        var controls =  'TextBox PasswordBox DataGrid DockPanel CheckBox RadioButton Button ListView Label TextBlock EditText TextView';

        var enums =     'AttributeTargets BindingFlags';

		function fixComments(match, regexInfo)
		{
			var css = (match[0].indexOf("///") == 0)
				? 'color3'
				: 'comments'
				;

			return [new SyntaxHighlighter.Match(match[0], match.index, css)];
		}

		function fixGenerics(match, regexInfo) {
		    return [new SyntaxHighlighter.Match(match[1], match.index + 4, 'color1')];
		}

		function findValue(m, regexInfo) {
		    var index = m.index,
		        match = null,
		        matches = [],
		        code = m[0];
		    var reg = /\bvalue\b/g;

		    while ((match = reg.exec(code)) != null) {
		        resultMatch = [new SyntaxHighlighter.Match(match[0], index + match.index, 'keyword')];

		        matches = matches.concat(resultMatch);
		    }
		    return matches;
		}

		function interpolatedString(m, regexInfo) {
		    var index = m.index,
		        match = null,
		        matches = [],
		        code = m[0],
		        cur = 0;
		    var reg = /[^{]{[^{][^{}]*}[^}]/g;

		    while ((match = reg.exec(code)) != null) {
		        var str = code.substring(cur, match.index + 1);
		        resultMatch = [new SyntaxHighlighter.Match(str, index + cur, 'string')];
		        cur = match.index + match[0].length - 1;

		        matches = matches.concat(resultMatch);
		    }
		    resultMatch = [new SyntaxHighlighter.Match(code.substring(cur, code.length), index + cur, 'string')];
		    matches = matches.concat(resultMatch);

		    return matches;
		}

		this.regexList = [
			{ regex: SyntaxHighlighter.regexLib.singleLineCComments,	func : fixComments },		// one line comments
			{ regex: SyntaxHighlighter.regexLib.multiLineCComments,		css: 'comments' },			// multiline comments
			{ regex: /\$@"(?:(?:[^{]?{[^{][^{}]+}(?=[^}]))|[^"]|"")*"/g, func: interpolatedString },	// $@-quoted strings
			{ regex: /\@"(?:[^"]|"")*"/g,								css: 'string' },			// @-quoted strings
			{ regex: /\$"((?:[^{]?{[^{][^{}]+}(?=[^}]))|[^\\"\n]|\\.)*"/g, func: interpolatedString },	// $-quoted strings
			{ regex: SyntaxHighlighter.regexLib.doubleQuotedString,		css: 'string' },			// strings
			{ regex: SyntaxHighlighter.regexLib.singleQuotedString,		css: 'string' },			// strings
			{ regex: /^\s*#.*/gm,										css: 'preprocessor' },		// preprocessor tags like #region and #endregion
			{ regex: new RegExp(this.getKeywords(keywords), 'gm'),		css: 'keyword' },			// c# keyword
			{ regex: /\bpartial(?=\s+(?:class|interface|struct)\b)/g,	css: 'keyword' },			// contextual keyword: 'partial'
			{ regex: /\byield(?=\s+(?:return|break)\b)/g,				css: 'keyword' },			// contextual keyword: 'yield'
			{ regex: /\bset\s+{(?:[^{}]|\n)+(?:{(?:[^{}]|\n)+(?:{(?:[^{}]|\n)+}(?:[^{}]|\n)+)?}(?:[^{}]|\n)+)?}/g, func: findValue },		// contextual keyword: 'value'
			{ regex: new RegExp(this.getKeywords(classes1), 'gm'),		css: 'color1' },			// c# common class A-D
			{ regex: new RegExp(this.getKeywords(classes2), 'gm'),		css: 'color1' },			// c# common class E-M
			{ regex: new RegExp(this.getKeywords(classes3), 'gm'),		css: 'color1' },			// c# common class N-R
			{ regex: new RegExp(this.getKeywords(classes4), 'gm'),		css: 'color1' },			// c# common class S-U
			{ regex: new RegExp(this.getKeywords(controls), 'gm'),		css: 'color1' },			// c# common controls in WPF, Form, Android
			{ regex: /\b[A-Z]\w*(?:Exception|Attribute)\b/g,            css: 'color1' },		    // c# exception and attribute
			{ regex: /&lt;([A-Z]\w+)&gt;/g,                             func: fixGenerics },        // c# generics <T>
			{ regex: /\bT\b/g,                                          css: 'color2' },            // c# generics T
			{ regex: /\bI[A-Z]\w+(?=\b)/g,                              css: 'color2' }, 			// c# interface
			{ regex: new RegExp(this.getKeywords(enums), 'gm'),         css: 'color2' }, 			// c# enums
			{ regex: /\b0x[0-9A-Fa-f]\b/g,                              css: 'value' }, 		    // c# hex value
			{ regex: /\b\d+(?:l|L|d|D|f|F)?\b/g,                        css: 'value' }, 		    // c# integer value
			{ regex: /\b\d*\.\d+(?:d|D|f|F)?\b/g,                       css: 'value' } 			    // c# decimal value
		];

		if (!(typeof NeetTeen === 'undefined')) {
		    if (!/^\s*$/.test(NeetTeen.CustomClass)) {
		        this.regexList.push({ regex: new RegExp(this.getKeywords(NeetTeen.CustomClass), 'gm'), css: 'color1' });
		    }
		    if (!/^\s*$/.test(NeetTeen.CustomEnum)) {
		        this.regexList.push({ regex: new RegExp(this.getKeywords(NeetTeen.CustomEnum), 'gm'), css: 'color2' });
		    }
		}
		
		this.forHtmlScript(SyntaxHighlighter.regexLib.aspScriptTags);
	};

	Brush.prototype	= new SyntaxHighlighter.Highlighter();
	Brush.aliases	= ['c#', 'c-sharp', 'csharp'];
	Brush.ext = '.cs';

	SyntaxHighlighter.brushes.CSharp = Brush;

	// CommonJS
	typeof(exports) != 'undefined' ? exports.Brush = Brush : null;
})();
