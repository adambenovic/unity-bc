from baron import parse, dumps
import json
from redbaron import RedBaron
import redbaron
import logging
import sys
import getopt
import sqd_element_creation as sec
import os


class JSONBuilder:
    logging.basicConfig(format='%(asctime)s - %(message)s', level=logging.DEBUG)

    def __init__(self, argv):
        inputfile = ''
        outputfile = ''

        try:
            opts, args = getopt.getopt(argv, "hi:o:m:", ["ifile=", "ofile=", "method="])
        except getopt.GetoptError:
            logging.error('test.py -i <inputfile> -o <outputfile> -m <method>')
            sys.exit(2)
        for opt, arg in opts:
            if opt == '-h':
                print('test.py -i <inputfile> -o <outputfile> -m <method>')
                sys.exit()
            elif opt in ("-i", "--ifile"):
                inputfile = arg
            elif opt in ("-o", "--ofile"):
                outputfile = arg
            elif opt in ("-m", "--method"):
                method = arg

        logging.info('Input file is %s', inputfile)
        logging.info('Output file is %s', outputfile)
        logging.info('Method is %s', method)
        self.inputFile = inputfile
        self.outputFile = outputfile
        self.method_to_parse = method

        self.nestingClasses = [redbaron.TryNode]
        self.lifelines = []
        self.imports = []
        self.class_variables = []
        self.messages = []
        self.output = []
        self.interaction = {}

    def build_json(self):
        self.inputFile = os.path.basename(self.inputFile)
        sec.create_new_interaction(self)
        sec.create_new_lifeline(self, self.inputFile[:-3])

        with open(self.inputFile, "r") as source_code:
            red = RedBaron(source_code.read())

            # red("decorator", lambda x: x.dumps() == "@decorator").map(lambda x: x.parent.parent.decorators.remove(x))

            for i in range(0, len(red)):
                if red[i].__class__ == redbaron.ImportNode or red[i].__class__ == redbaron.FromImportNode:
                    load_imports(self, red[i])

            logging.debug("Imports " + str(self.imports))

            red = red.find("def", name=self.method_to_parse)

            if red is None:
                logging.error('ERROR FINDING METHOD')
                sys.exit(2)

            for i in range(0, len(red)):
                process_node(self, red[i])

            with open(self.outputFile, 'w') as outfile: # ZMENIT NA data.json
                json.dump(self.output, outfile)


def process_node(self, red, new_fragments=None):
    if red.__class__ == redbaron.EndlNode:
        pass
    elif red.__class__ == redbaron.IfelseblockNode:
        new_fragments = process_ifelseblocknode(self, red, new_fragments)
    elif red.__class__ == redbaron.ReturnNode:
        pass
    elif red.__class__ == redbaron.CommentNode:
        pass
    elif red.__class__ == redbaron.ForNode or red.__class__ == redbaron.WhileNode:
        new_fragments = process_loop_node(self, red, new_fragments)
    elif red.__class__ == redbaron.TryNode:
        process_try_node(self, red)
    elif red.__class__ == redbaron.AssignmentNode:
        new_fragments = process_assignment_node(self, red, new_fragments)
    elif red.__class__ == redbaron.AtomtrailersNode:
        new_fragments = process_atomtrailer(self, red, new_fragments)

    return new_fragments


def process_loop_node(self, red, new_fragments_parameter):
    new_fragments = list()

    for i in range(0, len(red)):
        new_fragments = process_node(self, red[i], new_fragments)

    if red.__class__ == redbaron.ForNode:
        body = "for " + str(red.iterator) + " in " + str(red.target)
    elif red.__class__ == redbaron.WhileNode:
        if hasattr(red.test, 'first'):
            body = str(red.test.first) + " " + str(red.test.value) + " " + str(red.test.second)
        else:  # case when condition is single boolean e.g if needsConfirmation
            body = str(red.test)

    json_opaque_expression = sec.create_new_opaque_expression(body)
    json_interaction_constraint = sec.create_new_interaction_constraint(json_opaque_expression["XmiId"])
    json_interaction_operand = sec.create_new_interaction_operand(self, json_interaction_constraint["XmiId"],
                                                                  new_fragments)

    json_combined_fragment = sec.create_new_combined_fragment(self, json_interaction_operand["XmiId"], 7)
    json_interaction_operand["owner"] = {"XmiIdRef": json_combined_fragment["XmiId"]}

    self.output.append(json_interaction_operand)
    self.output.append(json_interaction_constraint)
    self.output.append(json_opaque_expression)
    self.output.append(json_combined_fragment)

    if new_fragments_parameter is not None:
        new_fragments_parameter.append(json_combined_fragment["XmiId"])

    return new_fragments_parameter


def process_ifelseblocknode(self, red, new_fragments):
    json_combined_fragment = {}

    for i in range(0, len(red.value)):
        if red.value[i].__class__ == redbaron.IfNode:
            json_interaction_operand = process_condition_node(self, red.value[i])
            json_combined_fragment = sec.create_new_combined_fragment(self, json_interaction_operand["XmiId"], 3)
            json_interaction_operand["owner"] = {"XmiIdRef": json_combined_fragment["XmiId"]}
            self.output.append(json_interaction_operand)

        elif red.value[i].__class__ == redbaron.ElifNode or red.value[i].__class__ == redbaron.ElseNode:
            json_combined_fragment["interactionOperator"] = 2
            json_interaction_operand = process_condition_node(self, red.value[i])
            json_interaction_operand["owner"] = {"XmiIdRef": json_combined_fragment["XmiId"]}
            json_combined_fragment["operand"].append({"XmiIdRef": json_interaction_operand["XmiId"]})
            json_combined_fragment["ownedElement"].append({"XmiIdRef": json_interaction_operand["XmiId"]})
            self.output.append(json_interaction_operand)

    self.output.append(json_combined_fragment)
    if new_fragments is not None:
        new_fragments.append(json_combined_fragment["XmiId"])

    return new_fragments


def process_condition_node(self, red):
    new_fragments = list()

    for i in range(0, len(red)):
        new_fragments = process_node(self, red[i], new_fragments)
    logging.debug("Nove fragmenty " + str(new_fragments))

    if type(red) == redbaron.ElseNode:
        body = "else"
    else:
        if hasattr(red.test, 'first'):
            body = str(red.test.first) + " " + str(red.test.value) + " " + str(red.test.second)
        else:  # case when condition is single boolean e.g if needsConfirmation
            body = str(red.test)

    json_opaque_expression = sec.create_new_opaque_expression(body)
    json_interaction_constraint = sec.create_new_interaction_constraint(json_opaque_expression["XmiId"])
    json_interaction_operand = sec.create_new_interaction_operand(self, json_interaction_constraint["XmiId"],
                                                                  new_fragments)
    self.output.append(json_interaction_constraint)
    self.output.append(json_opaque_expression)

    return json_interaction_operand


def process_try_node(self, red):
    for i in range(0, len(red)):
        process_node(self, red[i])


def process_assignment_node(self, red, new_fragments):
    if red.target not in self.class_variables:
        self.class_variables.append(red.target.value)

    if type(red.value) == redbaron.AtomtrailersNode:
        new_fragments = process_atomtrailer(self, red.value, new_fragments)

    return new_fragments


def process_atomtrailer(self, red, new_fragments):
    for item in red:
        if type(item) == redbaron.CallNode:
            class_name, method_name = get_method_details(self, red)

            if class_name not in self.lifelines:
                sec.create_new_lifeline(self, class_name)

            if method_name not in self.messages:
                json_oc_receive = sec.create_new_occurrence_specification(self, class_name, method_name)
                json_oc_send = sec.create_new_occurrence_specification(self, self.inputFile[:-3], method_name)
                json_message = sec.create_new_message(self, json_oc_receive["XmiId"], json_oc_send["XmiId"], method_name)

                logging.debug("Class: " + class_name + ", Method: " + method_name +
                              " " + json_oc_receive["XmiId"] + " " + json_oc_send["XmiId"])

                if new_fragments is not None:
                    new_fragments.append(json_oc_receive["XmiId"])
                    new_fragments.append(json_oc_send["XmiId"])

                self.output.append(json_message)
                self.output.append(json_oc_receive)
                self.output.append(json_oc_send)

    return new_fragments


def get_method_details(self, red_atomtrailer):
    method_name = ""
    class_name = ""

    ## CASE WHEN WE CALL FUNCTION DEFINED IN SAME CLASS, LEN of atomtrailer is exactly 2
    if len(red_atomtrailer) == 2:
        for item in red_atomtrailer:
            if type(item) == redbaron.NameNode:
                method_name = item.value
                class_name = self.inputFile[:-3]

    else:
        for item in red_atomtrailer:
            if type(item) == redbaron.NameNode:
                if item.value in self.class_variables:
                    class_name = self.inputFile
                    class_name = class_name[:-3]
                elif item.value not in self.imports:
                    method_name = method_name + item.value + "."
                else:
                    class_name = item.value

        method_name = method_name[:-1]

    return class_name, method_name


def load_imports(self, red):
    import_names = red.names()
    for item in import_names:
        if item not in self.imports:
            self.imports.append(item)


if __name__ == '__main__':
    logging.debug(sys.argv)
    jsonBuilder = JSONBuilder(sys.argv[1:])

    JSONBuilder.build_json(jsonBuilder)

